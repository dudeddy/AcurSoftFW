Imports System.Linq
Imports System.Threading
Imports AcurSoft.UFId.DAL
Imports AcurSoft.UFId.Utils
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Utils
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Xpo.DB.Exceptions
Imports DevExpress.Xpo.Metadata

Public Class SequenceGenerator
    Implements IDisposable

    Public Const MaxGenerationAttemptsCount As Integer = 10
    Public Const MinGenerationAttemptsDelay As Integer = 100
    'INSTANT VB TODO TASK: There is no VB equivalent to 'volatile':
    'ORIGINAL LINE: private static volatile IDataLayer defaultDataLayer;

    Private Shared _DefaultDataLayer As IDataLayer
    Private Shared _SyncRoot As New Object()
    Private euow As ExplicitUnitOfWork
    'Private seq As Sequence
    Private _UFIdSequences As List(Of UFIdSequence)
    Public Sub New(ByVal lockedSequenceTypes As Dictionary(Of String, Boolean))
        Dim count As Integer = MaxGenerationAttemptsCount
        Do
            Try
                euow = New ExplicitUnitOfWork(DefaultDataLayer)
                'Dennis: It is necessary to update all sequences because objects graphs may be complex enough, and so their sequences should be locked to avoid a deadlock.
                Dim sequences As New XPCollection(Of UFIdConfigLink)(euow, New InOperator(NameOf(UFIdConfigLink.TargetTypeName), lockedSequenceTypes.Keys))
                For Each seq As UFIdConfigLink In sequences
                    seq.Save()
                Next seq
                euow.FlushChanges()
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
        euow.CommitChanges()
    End Sub
    Public Sub Close()
        If euow IsNot Nothing Then
            If euow.InTransaction Then
                euow.RollbackTransaction()
            End If
            euow.Dispose()
            euow = Nothing
        End If
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Close()
    End Sub

    Public Function UFIdConfigLinks(ByVal targetTypeName As String) As XPCollection(Of UFIdConfigLink)
        'Guard.ArgumentNotNull(theObject, "theObject")
        'Dim ti As ITypeInfo = XafTypesInfo.Instance.FindTypeInfo(theObject.GetType())
        Return New XPCollection(Of UFIdConfigLink)(euow, CriteriaOperator.Parse(NameOf(UFIdConfigLink.TargetTypeName) & " = ?", targetTypeName))
    End Function
    Public Function UFIdConfigLinks(ByVal theObject As BaseObject) As XPCollection(Of UFIdConfigLink)
        Guard.ArgumentNotNull(theObject, "theObject")
        Dim ti As ITypeInfo = XafTypesInfo.Instance.FindTypeInfo(theObject.GetType())
        Return New XPCollection(Of UFIdConfigLink)(euow, CriteriaOperator.Parse(NameOf(UFIdConfigLink.TargetTypeName) & " = ?", ti.FullName))
    End Function

    Public Function GetLastUsedSequenceNumValues(ByVal theObject As BaseObject) As Dictionary(Of String, Long)
        Guard.ArgumentNotNull(theObject, "theObject")
        Return Me.UFIdConfigLinks(theObject)?.ToDictionary(Of String, Long)(Function(k) k.TargetMember, Function(v) v.LastUsedNumValue)
    End Function


    'Public Function GetNextSequence(ByVal typeInfo As ITypeInfo) As Long
    '    Guard.ArgumentNotNull(typeInfo, "typeInfo")
    '    Return GetNextSequence(XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary.GetClassInfo(typeInfo.Type))
    'End Function
    Public Function SetDraftSequenceValues(ByVal o As BaseObject) As List(Of UFIdSequenceInfos)
        'Guard.ArgumentNotNull(classInfo, "classInfo")
        'Dim ci As XPClassInfo = classInfo
        ''Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
        'Do While ci.BaseClass IsNot Nothing AndAlso ci.BaseClass.IsPersistent
        '    ci = ci.BaseClass
        'Loop
        Dim links = Me.UFIdConfigLinks(o.ClassInfo.FullName)
        'IsNewObject(This)
        Dim sequenceInfos As New List(Of UFIdSequenceInfos)
        For Each link In links
            sequenceInfos.Add(New UFIdSequenceInfos(o, link))
            o.SetMemberValue(link.TargetMember, link.GetDraftUFIdText(o))
        Next
        'seq = euow.GetObjectByKey(Of Sequence)(ci.FullName, True)
        'If seq Is Nothing Then
        '    Throw New InvalidOperationException(String.Format("Sequence for the {0} type was not found.", ci.FullName))
        'End If
        'Dim nextSequence As Long = seq.NextSequence
        'seq.NextSequence += 1
        'euow.FlushChanges()
        Return sequenceInfos
    End Function

    Public Function SetNextSequence(ByVal o As BaseObject) As List(Of UFIdSequence)
        'Guard.ArgumentNotNull(classInfo, "classInfo")
        'Dim ci As XPClassInfo = classInfo
        ''Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
        'Do While ci.BaseClass IsNot Nothing AndAlso ci.BaseClass.IsPersistent
        '    ci = ci.BaseClass
        'Loop
        Dim links = Me.UFIdConfigLinks(o.ClassInfo.FullName)

        _UFIdSequences = New List(Of UFIdSequence)
        For Each link In links
            If link.CanApplySequence(o) Then


                Dim seq As New UFIdSequence(euow) With {
                .TargetId = o.Oid,
                .Link = link,
                .Status = UFId.Enums.SequenceStatuses.Assigned,
                .DateValue = link.GetUFIdDateValue(o).Value,
                .UFIdDateText = link.GetUFIdDateText(.DateValue),
                .NumValue = link.GetDraftNumValue(.UFIdDateText),
                .UFIdText = link.GetDraftUFIdText(o, .DateValue, .UFIdDateText, .NumValue)
            }
                _UFIdSequences.Add(seq)
                o.SetMemberValue(link.TargetMember, seq.UFIdText)
                link.LastUsedNumValue = seq.NumValue
                link.LastUsedDateValue = seq.DateValue
            End If

        Next
        'seq = euow.GetObjectByKey(Of Sequence)(ci.FullName, True)
        'If seq Is Nothing Then
        '    Throw New InvalidOperationException(String.Format("Sequence for the {0} type was not found.", ci.FullName))
        'End If
        'Dim nextSequence As Long = seq.NextSequence
        'seq.NextSequence += 1
        euow.FlushChanges()
        Return _UFIdSequences
    End Function
    Public Shared ReadOnly Property DefaultDataLayer() As IDataLayer
        Get
            SyncLock _SyncRoot
                If _DefaultDataLayer Is Nothing Then
                    Dim objectSpaceProvider As XPObjectSpaceProvider = TryCast(UFIdGeneratorInitializer.Application.ObjectSpaceProvider, XPObjectSpaceProvider)
                    Guard.ArgumentNotNull(objectSpaceProvider, "XPObjectSpaceProvider")
                    If objectSpaceProvider.DataLayer Is Nothing Then
                        'Dennis: This call is necessary to initialize a working data layer if it is not yet ready.
                        objectSpaceProvider.CreateObjectSpace()
                    End If
                    If TypeOf objectSpaceProvider.DataLayer Is ThreadSafeDataLayer Then
                        'Dennis: We have to use a separate datalayer for the sequence generator because ThreadSafeDataLayer is usually used for ASP.NET applications.
                        _DefaultDataLayer = XpoDefault.GetDataLayer(DefaultDataLayerConnectionString, XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary, AutoCreateOption.None)
                    Else
                        _DefaultDataLayer = objectSpaceProvider.DataLayer
                    End If
                End If
                Return _DefaultDataLayer
            End SyncLock
        End Get
    End Property



    'Dennis: It is necessary to generate (only once) sequences for all the persistent types before using the GetNextSequence method.
    'Public Shared Sub RegisterSequences(ByVal persistentTypes As IEnumerable(Of ITypeInfo))
    '    If persistentTypes IsNot Nothing Then
    '        Using uow As New UnitOfWork(DefaultDataLayer)
    '            Dim sequenceList As New XPCollection(Of UFIdConfigLink)(uow)
    '            'Dim typeToExistsMap As New Dictionary(Of String, Boolean)()
    '            'Dim sequenceList As New XPCollection(Of UFIdConfigLink)(uow)
    '            Dim typeToExistsMap As Dictionary(Of String, Boolean) =
    '                sequenceList.Select(Function(q) q.TargetTypeName).Distinct.ToDictionary(Of String, Boolean)(Function(q) q, Function(q) True)

    '            persistentTypes = (From p In persistentTypes
    '                               Where Not typeToExistsMap.ContainsKey(p.FullName)).ToList

    '            'For Each typeInfo As ITypeInfo In persistentTypes
    '            '    Dim ti As ITypeInfo = typeInfo
    '            '    'Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
    '            '    Dim typeName As String = ti.FullName
    '            '    'Dennis: This code is required for the Domain Components only.
    '            '    If typeToExistsMap.ContainsKey(typeName) Then
    '            '        Continue For
    '            '    End If
    '            '    typeToExistsMap(typeName) = True
    '            '    If ti.Type.Implements(GetType(ISupportUFIdBase)) Then
    '            '        Dim seq As New UFIdBase(uow) With {.typeName = typeName}
    '            '    End If
    '            'Next typeInfo
    '            'uow.CommitChanges()


    '            'For Each seq As UFIdConfigLink In sequenceList
    '            '    'If typeToExistsMap.ContainsKey() Then
    '            '    typeToExistsMap(seq.TargetTypeName) = True
    '            'Next seq
    '            'For Each typeInfo As ITypeInfo In persistentTypes
    '            '    Dim ti As ITypeInfo = typeInfo
    '            '    If typeToExistsMap.ContainsKey(ti.FullName) Then
    '            '        Continue For
    '            '    End If
    '            '    'Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
    '            '    Do While ti.Base IsNot Nothing AndAlso ti.Base.IsPersistent
    '            '        ti = ti.Base
    '            '    Loop
    '            '    Dim typeName As String = ti.FullName
    '            '    'Dennis: This code is required for the Domain Components only.
    '            '    If ti.IsInterface AndAlso ti.IsPersistent Then
    '            '        Dim generatedEntityType As Type = XpoTypesInfoHelper.GetXpoTypeInfoSource().GetGeneratedEntityType(ti.Type)
    '            '        If generatedEntityType IsNot Nothing Then
    '            '            typeName = generatedEntityType.FullName
    '            '        End If
    '            '    End If
    '            '    If typeToExistsMap.ContainsKey(typeName) Then
    '            '        Continue For
    '            '    End If
    '            '    If ti.IsPersistent Then
    '            '        typeToExistsMap(typeName) = True
    '            '        Dim seq As New Sequence(uow)
    '            '        seq.TypeName = typeName
    '            '        seq.NextSequence = 0
    '            '    End If
    '            'Next typeInfo
    '            'uow.CommitChanges()
    '        End Using
    '    End If
    'End Sub
    ''Dennis: It is important to set the SequenceGenerator.DefaultDataLayer property to the main application data layer.
    'If you use a custom IObjectSpaceProvider implementation, ensure that it exposes a working IDataLayer.

    'Private Shared _DefaultDataLayer As IDataLayer
    'Public Shared Property DefaultDataLayer() As IDataLayer
    '    Get
    '        If _DefaultDataLayer Is Nothing Then
    '            Throw New ArgumentNullException("DefaultDataLayer")
    '        End If
    '        Return _DefaultDataLayer
    '    End Get
    '    Set(ByVal value As IDataLayer)
    '        SyncLock _SyncRoot
    '            _DefaultDataLayer = value
    '        End SyncLock
    '    End Set
    'End Property

    'Public Shared ReadOnly Property DefaultDataLayer() As IDataLayer
    '    Get
    '        SyncLock syncRoot
    '            If defaultDataLayer_Renamed Is Nothing Then
    '                Dim objectSpaceProvider As XPObjectSpaceProvider = TryCast(TargetApplication.ObjectSpaceProvider, XPObjectSpaceProvider)
    '                Guard.ArgumentNotNull(objectSpaceProvider, "XPObjectSpaceProvider")
    '                If objectSpaceProvider.DataLayer Is Nothing Then
    '                    'Dennis: This call is necessary to initialize a working data layer if it is not yet ready.
    '                    objectSpaceProvider.CreateObjectSpace()
    '                End If
    '                If TypeOf objectSpaceProvider.DataLayer Is ThreadSafeDataLayer Then
    '                    'Dennis: We have to use a separate datalayer for the sequence generator because ThreadSafeDataLayer is usually used for ASP.NET applications.
    '                    defaultDataLayer_Renamed = XpoDefault.GetDataLayer(DefaultDataLayerConnectionString, XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary, AutoCreateOption.None)
    '                Else
    '                    defaultDataLayer_Renamed = objectSpaceProvider.DataLayer
    '                End If
    '            End If
    '            Return defaultDataLayer_Renamed
    '        End SyncLock
    '    End Get
    'End Property
    Public Shared Sub Initialize(ByVal targetApplication As XafApplication, ByVal defaultDataLayerConnectionString As String)
        Guard.ArgumentNotNullOrEmpty(defaultDataLayerConnectionString, "defaultDataLayerConnectionString")
        If SequenceGenerator.DefaultDataLayerConnectionString Is Nothing Then
            SequenceGenerator.DefaultDataLayerConnectionString = defaultDataLayerConnectionString
        End If
        Guard.ArgumentNotNull(targetApplication, "targetApplication")
        If SequenceGenerator.TargetApplication Is Nothing Then
            SequenceGenerator.TargetApplication = targetApplication
        End If
    End Sub
    Private Shared privateDefaultDataLayerConnectionString As String
    Protected Shared Property DefaultDataLayerConnectionString() As String
        Get
            Return privateDefaultDataLayerConnectionString
        End Get
        Private Set(ByVal value As String)
            privateDefaultDataLayerConnectionString = value
        End Set
    End Property
    Private Shared privateTargetApplication As XafApplication
    Protected Shared Property TargetApplication() As XafApplication
        Get
            Return privateTargetApplication
        End Get
        Private Set(ByVal value As XafApplication)
            privateTargetApplication = value
        End Set
    End Property
End Class
