' Developer Express Code Central Example:
' How to generate and assign a sequential number for a business object within a database transaction, while being a part of a successful saving process (XAF)
' 
' This version of the http://www.devexpress.com/scid=E2620 example is primarily
' intended for XAF applications, because all the required operations to generate
' sequences are managed within the base persistent class. However, this code can
' be used in a regular non-XAF application based on XPO, as well.
' For more
' convenience, this solution is organized as a reusable module -
' GenerateUserFriendlyId.Module, which you may want to copy into your project 'as
' is' and then add it as required via the Module or Application designers
' (http://documentation.devexpress.com/#Xaf/CustomDocument2828). This module
' consists of several key parts:
' 1. Sequence, SequenceGenerator, and
' SequenceGeneratorInitializer - auxiliary classes that take the main part in
' generating user-friendly identifiers.
' Take special note that the last two
' classes need to be initialized in the ModuleUpdater and ModuleBase descendants
' of your real project.
' 
' 2. UserFriendlyIdPersistentObject - a base persistent
' class that subscribes to XPO's Session events and delegates calls to the core
' classes above.
' 
' 3. IUserFriendlyIdDomainComponent - a base domain component
' that should be implemented by all domain components that require the described
' functionality. Take special note that such derived components must still use the
' BasePersistentObject as a base class during the registration, e.g.:
' XafTypesInfo.Instance.RegisterEntity("Document", typeof(IDocument),
' typeof(BasePersistentObject)); 4. Address, Contact, and IDocument are business
' objects that demonstrate the use of the described functionality for XPO and DC
' respectively.
' Take special note that a required format for the user-friendly
' identifier property in these end classes is defined within an aliased property
' (AddressId in the example below) by concatenation of a required constant or
' dynamic value with the <i>SequentialNumber</i> property provided by the base
' <i>UserFriendlyIdPersistentObject</i> class. So, if you need to have a different
' format, modify the PersistentAliasAttribute expression as your business needs
' dictate:    [PersistentAlias("concat('A', ToStr(SequentialNumber))")]    public
' string AddressId {      get {  return
' Convert.ToString(EvaluateAlias("AddressId")); }    }
' 
' 
' 
' IMPORTANT NOTES
' 1.
' If your connection string contains the Password parameter, then rework the
' SequenceGeneratorInitializer.Initialize method to not use the
' <i>XafApplication.ConnectionString</i> or
' <i>XafApplication.Connection.ConnectionString</i> properties, because XAF
' encrypts the specified password for safety reasons. Instead, either specify the
' connection string directly or read it from the configuration file.
' 2. The
' sequential number functionality shown in this example does not work with shared
' parts
' (http://documentation.devexpress.com/#Xaf/DevExpressExpressAppDCITypesInfo_RegisterSharedParttopic)
' (a part of the Domain Components (DC) technology) in the current version,
' because it requires a custom base class, which is not allowed for shared
' parts.
' 3. This solution is not yet tested in the middle-tier and
' SecuredObjectSpaceProvider scenario and most likely, it will have to be modified
' to support its specifics.
' 4. As an alternative, you can use a more simple
' solution that is using the <i>DistributedIdGeneratorHelper.Generate</i> method
' as shown in the FeatureCenter demo (<i>"%Public%\Documents\DXperience 13.X
' Demos\eXpressApp
' Framework\FeatureCenter\CS\FeatureCenter.Module\KeyProperty\GuidKeyPropertyObject.cs"</i>
' ) or at http://www.devexpress.com/scid=E4904
' 
' You can find sample updates and versions for different programming languages here:
' http://www.devexpress.com/example=E2829

Imports DevExpress.Xpo
Imports System.Threading
Imports DevExpress.ExpressApp
Imports DevExpress.Xpo.Metadata
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpo.DB.Exceptions
Imports DevExpress.ExpressApp.Utils
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.Persistent.BaseImpl
Imports Fasterflect
Imports AcurXAF.Data.BOs
Imports AcurXAF.UFId.Data.BOs
Imports AcurXAF.UFId.Utils.Helpers
Imports AcurXAF.UFId.Data.BOs.Core
Imports AcurXAF.UFId.Core.Helpers

'Namespace UFId.Core
'Dennis: This class is used to generate sequential numbers for persistent objects.
'Use the GetNextSequence method to get the next number and the Accept method, to save these changes to the database.
Public Class UFIdGenerator
        Implements IDisposable
        Public Const MaxGenerationAttemptsCount As Integer = 10
        Public Const MinGenerationAttemptsDelay As Integer = 100
        Private Shared _SyncRoot As New Object()
        Private _ExplicitUnitOfWork As ExplicitUnitOfWork
        Private _Sequence As UFIdBase
        Private _StroreMissing As Boolean

        Private Shared _DefaultDataLayer As IDataLayer
        Public Shared Property DefaultDataLayer() As IDataLayer
            Get
                If _DefaultDataLayer Is Nothing Then
                    Throw New ArgumentNullException("DefaultDataLayer")
                End If
                Return _DefaultDataLayer
            End Get
            Set(ByVal value As IDataLayer)
                SyncLock _SyncRoot
                    _DefaultDataLayer = value
                End SyncLock
            End Set
        End Property

        Public Sub New(ByVal lockedSequenceTypes As Dictionary(Of String, Boolean))
            Dim count As Integer = MaxGenerationAttemptsCount
            Do
                Try
                    _ExplicitUnitOfWork = New ExplicitUnitOfWork(UFIdGenerator.DefaultDataLayer)
                    'Dennis: It is necessary to update all sequences because objects graphs may be complex enough, and so their sequences should be locked to avoid a deadlock.
                    Dim sequences As New XPCollection(Of UFIdBase)(_ExplicitUnitOfWork, New InOperator("TypeName", lockedSequenceTypes.Keys), New SortProperty("TypeName", DevExpress.Xpo.DB.SortingDirection.Ascending))
                    For Each seq In sequences
                        seq.Save()
                    Next seq
                    _ExplicitUnitOfWork.FlushChanges()
                    Exit Do
                Catch e1 As LockingException
                    Close()
                    count -= 1
                    If count <= 0 Then
                        Throw
                    End If
                    Thread.Sleep(MinGenerationAttemptsDelay * count)
                End Try
            Loop
        End Sub
        Public Sub Accept()
            _ExplicitUnitOfWork.CommitChanges()
        End Sub
        Public Sub Close()
            If _ExplicitUnitOfWork IsNot Nothing Then
                If _ExplicitUnitOfWork.InTransaction Then
                    _ExplicitUnitOfWork.RollbackTransaction()
                End If
                _ExplicitUnitOfWork.Dispose()
                _ExplicitUnitOfWork = Nothing
            End If
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Close()
        End Sub

        Public Function SetSequence(ci As XPClassInfo) As UFIdBase
            If ci.ClassType.Implements(GetType(ISupportUFIdBase)) Then
                _Sequence = _ExplicitUnitOfWork.FindObject(Of UFIdBase)(CriteriaOperator.Parse("TypeName = ?", ci.FullName))
            End If
            _StroreMissing = _Sequence.StoreMissing
            Return _Sequence
        End Function


        Public Sub PrepareOperation(ByVal o As BaseObject, updateValue As Boolean)
            If o Is Nothing Then
                Throw New ArgumentNullException("theObject")
            End If
            Dim ci As XPClassInfo = o.ClassInfo
            'Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.

            Me.SetSequence(ci)
            If _Sequence Is Nothing Then
                Dim ti As ITypeInfo = XafTypesInfo.Instance.FindTypeInfo(ci.ClassType)
                UFIdGenerator.RegisterSequences({ti}.ToList)
                Me.SetSequence(ci)
                If _Sequence Is Nothing Then
                    Throw New InvalidOperationException(String.Format("Sequence for the {0} type was not found.", ci.FullName))
                End If
            End If
        End Sub

        Public Function DoOperation(Of T)(
                ByVal o As BaseObject,
                updateValue As Boolean,
                f As Func(Of BaseObject, Boolean, T)) As T
            If o Is Nothing Then
                Throw New ArgumentNullException("theObject")
            End If
            Dim ci As XPClassInfo = o.ClassInfo
            'Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.

            Me.SetSequence(ci)
            If _Sequence Is Nothing Then
                Dim ti As ITypeInfo = XafTypesInfo.Instance.FindTypeInfo(ci.ClassType)
                UFIdGenerator.RegisterSequences({ti}.ToList)
                Me.SetSequence(ci)
                If _Sequence Is Nothing Then
                    Throw New InvalidOperationException(String.Format("Sequence for the {0} type was not found.", ci.FullName))
                End If
            End If
            Return f.Invoke(o, updateValue)
        End Function

        Public Function GetNextSequence(ByVal o As BaseObject, updateValue As Boolean) As UFIdBaseInfos
            Me.PrepareOperation(o, updateValue)
            Dim seq_info As UFIdBaseInfos = _Sequence.GetUFIdInfos(o)
            If updateValue Then
                seq_info.SetSequenceNextValues()
                If seq_info.MissingUsed Then
                    _ExplicitUnitOfWork.Delete(seq_info.GetMissingItemUsed(_ExplicitUnitOfWork))
                End If
                _ExplicitUnitOfWork.FlushChanges()
            Else
                Me.Close()
            End If
            Return seq_info
        End Function
        'Dennis: It is necessary to generate (only once) sequences for all the persistent types before using the GetNextSequence method.
        Public Shared Sub RegisterSequences(ByVal persistentTypes As List(Of ITypeInfo))
            'TODO to be optimized
            If persistentTypes Is Nothing Then Return
            Using uow As New UnitOfWork(UFIdGenerator.DefaultDataLayer)
                Dim sequenceList As New XPCollection(Of UFIdBase)(uow)
                Dim typeToExistsMap As Dictionary(Of String, Boolean) =
                    sequenceList.Select(Function(q) q.TypeName).Distinct.ToDictionary(Of String, Boolean)(Function(q) q, Function(q) True)

                persistentTypes = (From p In persistentTypes
                                   Where Not typeToExistsMap.ContainsKey(p.FullName)).ToList

                For Each typeInfo As ITypeInfo In persistentTypes
                    Dim ti As ITypeInfo = typeInfo
                    'Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
                    Dim typeName As String = ti.FullName
                    'Dennis: This code is required for the Domain Components only.
                    If typeToExistsMap.ContainsKey(typeName) Then
                        Continue For
                    End If
                    typeToExistsMap(typeName) = True
                    If ti.Type.Implements(GetType(ISupportUFIdBase)) Then
                        Dim seq As New UFIdBase(uow) With {.typeName = typeName}
                    End If
                Next typeInfo
                uow.CommitChanges()
            End Using
        End Sub


        Public Shared Function GetLastNumValueOfComplexUFId(ByVal o As BaseObject, numField As String, complexValueField As String, complexValue As String) As Long?
            Dim cr As CriteriaOperator = CriteriaOperator.Parse(
            String.Format("Max(Iif({0} == ?,{1},0))", complexValueField, numField), complexValue)

            Dim l As Long?
            Using euow = New ExplicitUnitOfWork(UFIdGenerator.DefaultDataLayer)
                l = CType(euow.Evaluate(o.ClassInfo, cr, Nothing), Long?)
            End Using
            Return l
        End Function


        Public Shared Function GetMissingNumValue(ByVal o As UFIdBase, aggregate As Aggregate, Optional condition As CriteriaOperator = Nothing) As Long?
            Dim cr As CriteriaOperator '= CriteriaOperator.Parse(String.Format("UFIdMissingItems.{0}(UFIdNumber)", aggregate))
            If condition Is Nothing Then
                cr = New AggregateOperand("UFIdMissingItems", "UFIdNumber", aggregate)
            Else
                cr = New AggregateOperand("UFIdMissingItems", "UFIdNumber", aggregate, condition)
            End If
            Dim l As Long?
            Using euow = New ExplicitUnitOfWork(UFIdGenerator.DefaultDataLayer)
                l = CType(euow.GetObjectByKey(Of UFIdBase)(o.Oid).Evaluate(cr), Long?)
            End Using
            Return l
        End Function

        Public Function DeleteSequence(ByVal o As BaseObject, updateValue As Boolean) As UFIdMissingItem
            Me.PrepareOperation(o, updateValue)
            Dim missing_item As UFIdMissingItem = Nothing
            If _StroreMissing Then
                If updateValue Then
                    missing_item = New UFIdMissingItem(_Sequence, o) With {.UFIdIsDeleted = True}
                    _ExplicitUnitOfWork.FlushChanges()
                Else
                    Me.Close()
                End If
            Else
                Me.Close()
            End If
            Return missing_item
        End Function
    End Class
'This persistent class is used to store last sequential number for persistent objects.
'End Namespace