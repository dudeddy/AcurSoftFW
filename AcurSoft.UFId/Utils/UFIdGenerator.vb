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

Namespace UFId.Utils
    Public Class UFIdGenerator
        Implements IDisposable

        Public Const MaxGenerationAttemptsCount As Integer = 10
        Public Const MinGenerationAttemptsDelay As Integer = 100
        'INSTANT VB TODO TASK: There is no VB equivalent to 'volatile':
        'ORIGINAL LINE: private static volatile IDataLayer defaultDataLayer;

        Protected Shared Property DefaultDataLayerConnectionString() As String
        Protected Shared Property TargetApplication() As XafApplication

        Private Shared _DefaultDataLayer As IDataLayer
        Private Shared _SyncRoot As New Object()
        Private _Euow As ExplicitUnitOfWork
        Private _UFIdSequences As List(Of UFIdSequence)

        Public ReadOnly Property IsActive As Boolean
        Public ReadOnly Property IsInitiated As Boolean
        Public ReadOnly Property Links As XPCollection(Of UFId.DAL.UFIdConfigLink)
        Public Sub New(ByVal o As BaseObject)
            _Euow = New ExplicitUnitOfWork(DefaultDataLayer)
            Me.Links = Me.UFIdConfigLinks(o.ClassInfo.FullName)
            Me.IsActive = Links.Count > 0

        End Sub
        Public Sub Init(ByVal lockedSequenceTypes As Dictionary(Of String, Boolean))
            Dim count As Integer = MaxGenerationAttemptsCount
            Do
                Try
                    If _Euow Is Nothing Then
                        _Euow = New ExplicitUnitOfWork(DefaultDataLayer)
                    End If
                    'Dennis: It is necessary to update all sequences because objects graphs may be complex enough, and so their sequences should be locked to avoid a deadlock.
                    Dim sequences As New XPCollection(Of UFIdConfigLink)(_Euow, New InOperator(NameOf(UFIdConfigLink.TargetTypeName), lockedSequenceTypes.Keys))
                    'Dim sequences As New XPCollection(Of UFIdConfigLink)(euow, New InOperator(NameOf(UFIdConfigLink.TargetTypeName), lockedSequenceTypes.Keys))
                    'sequences As New XPCollection(Of UFIdConfigLink)(euow, New InOperator(NameOf(UFIdConfigLink.TargetTypeName), lockedSequenceTypes.Keys))

                    For Each seq As UFIdConfigLink In sequences
                        seq.Save()
                    Next seq
                    _Euow.FlushChanges()
                    _IsInitiated = True
                    _IsActive = True
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

        Public Sub New(ByVal lockedSequenceTypes As Dictionary(Of String, Boolean))
            Me.Init(lockedSequenceTypes)
        End Sub
        Public Sub Accept()
            _Euow.CommitChanges()
        End Sub
        Public Sub Close()
            If _Euow IsNot Nothing Then
                If _Euow.InTransaction Then
                    _Euow.RollbackTransaction()
                End If
                _Euow.Dispose()
                _Euow = Nothing
            End If
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            Close()
        End Sub

        'Public Property Link

        Public Function UFIdConfigLinks(ByVal targetTypeName As String) As XPCollection(Of UFIdConfigLink)
            Return New XPCollection(Of UFIdConfigLink)(_Euow, CriteriaOperator.Parse(NameOf(UFIdConfigLink.TargetTypeName) & " = ?", targetTypeName))
        End Function
        Public Function UFIdConfigLinks(ByVal theObject As BaseObject) As XPCollection(Of UFIdConfigLink)
            Guard.ArgumentNotNull(theObject, "theObject")
            Dim ti As ITypeInfo = XafTypesInfo.Instance.FindTypeInfo(theObject.GetType())
            Return New XPCollection(Of UFIdConfigLink)(_Euow, CriteriaOperator.Parse(NameOf(UFIdConfigLink.TargetTypeName) & " = ?", ti.FullName))
        End Function

        Public Function GetLastUsedSequenceNumValues(ByVal theObject As BaseObject) As Dictionary(Of String, Long)
            Guard.ArgumentNotNull(theObject, "theObject")
            Return Me.UFIdConfigLinks(theObject)?.ToDictionary(Of String, Long)(Function(k) k.TargetMember, Function(v) v.LastUsedNumValue)
        End Function

        Public Function SetDraftSequenceValues(ByVal o As BaseObject) As List(Of UFIdSequenceInfos)
            'Guard.ArgumentNotNull(classInfo, "classInfo")
            'Dim ci As XPClassInfo = classInfo
            ''Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
            'Do While ci.BaseClass IsNot Nothing AndAlso ci.BaseClass.IsPersistent
            '    ci = ci.BaseClass
            'Loop
            If Me.Links Is Nothing Then
                _Links = Me.UFIdConfigLinks(o.ClassInfo.FullName)
            End If
            'IsNewObject(This)
            Dim sequenceInfos As New List(Of UFIdSequenceInfos)
            For Each link In Me.Links
                Dim seqInfos As New UFIdSequenceInfos(o, link)

                sequenceInfos.Add(seqInfos)
                o.SetMemberValue(link.TargetMember, seqInfos.GetUFIdText())
            Next
            Return sequenceInfos
        End Function

        Public Function SetSequenceAsDeleted(ByVal o As BaseObject) As List(Of UFIdSequence)
            If Me.Links Is Nothing Then
                _Links = Me.UFIdConfigLinks(o.ClassInfo.FullName)
            End If

            _UFIdSequences = New List(Of UFIdSequence)
            For Each link In Me.Links
                Dim seq As UFIdSequence = link.UFIdSequences.FirstOrDefault(Function(q) q.TargetId = o.Oid)
                If seq IsNot Nothing Then
                    seq.Status = UFId.Enums.SequenceStatuses.Deleted
                    'seq.Save()
                    _UFIdSequences.Add(seq)
                End If

            Next

            _Euow.CommitTransaction()
            Return _UFIdSequences
        End Function


        Public Function CanApplySequence(link As UFId.DAL.UFIdConfigLink, o As BaseObject) As Boolean
            Return o.Fit(link.AppyCondition)
        End Function


        Public Function SetNextSequence(ByVal o As BaseObject) As List(Of UFIdSequenceInfos)
            'Guard.ArgumentNotNull(classInfo, "classInfo")
            'Dim ci As XPClassInfo = classInfo
            ''Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
            'Do While ci.BaseClass IsNot Nothing AndAlso ci.BaseClass.IsPersistent
            '    ci = ci.BaseClass
            'Loop
            'Dim links = Me.UFIdConfigLinks(o.ClassInfo.FullName)
            If Me.Links Is Nothing Then
                _Links = Me.UFIdConfigLinks(o.ClassInfo.FullName)
            End If
            _UFIdSequences = New List(Of UFIdSequence)
            Dim sequenceInfosList As New List(Of UFIdSequenceInfos)

            For Each link In Me.Links
                If Me.CanApplySequence(link, o) Then
                    Dim sequenceInfos As New UFIdSequenceInfos(o, link)
                    sequenceInfosList.Add(sequenceInfos)
                    If sequenceInfos.ValueInfos.NeedToAsk Then
                    Else
                        Dim seq As UFIdSequence = sequenceInfos.CreateSequence(_Euow)
                        _UFIdSequences.Add(seq)
                        o.SetMemberValue(link.TargetMember, seq.UFIdText)
                        link.LastUsedNumValue = seq.NumValue
                        link.LastUsedDateValue = seq.DateValue

                    End If
                End If

            Next
            _Euow.FlushChanges()
            Return sequenceInfosList
        End Function
        Public Shared ReadOnly Property DefaultDataLayer() As IDataLayer
            Get
                SyncLock _SyncRoot
                    If _DefaultDataLayer Is Nothing Then
                        Dim objectSpaceProvider As XPObjectSpaceProvider = TryCast(UFIdInitializer.Application.ObjectSpaceProvider, XPObjectSpaceProvider)
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

        Public Shared Sub Initialize(ByVal targetApplication As XafApplication, ByVal defaultDataLayerConnectionString As String)
            Guard.ArgumentNotNullOrEmpty(defaultDataLayerConnectionString, "defaultDataLayerConnectionString")
            If UFIdGenerator.DefaultDataLayerConnectionString Is Nothing Then
                UFIdGenerator.DefaultDataLayerConnectionString = defaultDataLayerConnectionString
            End If
            Guard.ArgumentNotNull(targetApplication, "targetApplication")
            If UFIdGenerator.TargetApplication Is Nothing Then
                UFIdGenerator.TargetApplication = targetApplication
            End If
        End Sub


    End Class
End Namespace
