Imports System
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.Persistent.Base
Imports AcurSoft.Enums

Namespace UFId.DAL

    <DefaultClassOptions>
    Partial Public Class TestObject
        Private Shared _SequenceGenerator As SequenceGenerator
        Private Shared _SyncRoot As New Object()

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
            Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.JustCreated))
            Me.SetDraftSequenceValues()
            'Dim links As New XPCollection(Of UFId.DAL.UFIdConfigLink)(Me.Session, CriteriaOperator.Parse("TargetTypeName = ?", Me.ClassInfo.FullName))
            ''Dim lll As New List(Of String)
            'For Each link In links
            '    Me.SetMemberValue(link.TargetMember, link.GetDraftUFIdText(Me))
            '    'lll.Add(l.GetDraftUFIdText(x))
            'Next

        End Sub

#Region "Sequence original"

        'Private Sub OnSequenceGenerated(ByVal newId As Long)
        '    _SequentialNumber = newId
        'End Sub
        Protected Overrides Sub OnSaving()
            Dim editStautusOriginal As ObjectEditStatuses = Me.EditStatus.Status
            Try
                MyBase.OnSaving()
                If editStautusOriginal = ObjectEditStatuses.JustCreated Then
                    Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.NewSaved))
                End If
                'If Not (TypeOf Session Is NestedUnitOfWork) AndAlso (Session.DataLayer IsNot Nothing) AndAlso (TypeOf Session.ObjectLayer Is SimpleObjectLayer) AndAlso Session.IsNewObject(Me) Then
                If Not (TypeOf Session Is NestedUnitOfWork) AndAlso (Session.DataLayer IsNot Nothing) AndAlso (TypeOf Session.ObjectLayer Is SimpleObjectLayer) Then
                    'OR
                    '&& !(Session.ObjectLayer is DevExpress.ExpressApp.Security.ClientServer.SecuredSessionObjectLayer)
                    Dim co As CriteriaOperator = CriteriaOperator.Parse("EditStatus.Status = ?", ObjectEditStatuses.Draft)
                    GenerateSequence()
                End If
            Catch
                If editStautusOriginal = ObjectEditStatuses.JustCreated Then
                    Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.JustCreated))
                End If

                CancelSequence()
                Throw
            End Try
        End Sub

        Public Sub CreateSequenceGenerator()
            SyncLock _SyncRoot
                If _SequenceGenerator Is Nothing Then
                    Dim typeToExistsMap As New Dictionary(Of String, Boolean)()
                    For Each item As Object In Session.GetObjectsToSave()
                        typeToExistsMap(Session.GetClassInfo(item).FullName) = True
                    Next item
                    _SequenceGenerator = New SequenceGenerator(typeToExistsMap)
                End If
            End SyncLock
        End Sub

        Public Sub SetDraftSequenceValues()
            Me.CreateSequenceGenerator()

            SyncLock _SyncRoot
                SubscribeToEvents()
                'OnSequenceGenerated(_SequenceGenerator.GetNextSequence(Me))
                _SequenceGenerator.SetDraftSequenceValues(Me)
            End SyncLock
            'SyncLock _SyncRoot
            '    If _SequenceGenerator Is Nothing Then
            '        Dim typeToExistsMap As New Dictionary(Of String, Boolean)()
            '        For Each item As Object In Session.GetObjectsToSave()
            '            typeToExistsMap(Session.GetClassInfo(item).FullName) = True
            '        Next item
            '        _SequenceGenerator = New SequenceGenerator(typeToExistsMap)
            '    End If
            '    SubscribeToEvents()
            '    'OnSequenceGenerated(_SequenceGenerator.GetNextSequence(Me))
            '    _SequenceGenerator.SetNextSequence(Me)
            'End SyncLock
        End Sub

        Public Sub GenerateSequence()
            Me.CreateSequenceGenerator()

            SyncLock _SyncRoot
                SubscribeToEvents()
                'OnSequenceGenerated(_SequenceGenerator.GetNextSequence(Me))
                _SequenceGenerator.SetNextSequence(Me)
            End SyncLock
            'SyncLock _SyncRoot
            '    If _SequenceGenerator Is Nothing Then
            '        Dim typeToExistsMap As New Dictionary(Of String, Boolean)()
            '        For Each item As Object In Session.GetObjectsToSave()
            '            typeToExistsMap(Session.GetClassInfo(item).FullName) = True
            '        Next item
            '        _SequenceGenerator = New SequenceGenerator(typeToExistsMap)
            '    End If
            '    SubscribeToEvents()
            '    'OnSequenceGenerated(_SequenceGenerator.GetNextSequence(Me))
            '    _SequenceGenerator.SetNextSequence(Me)
            'End SyncLock
        End Sub
        Private Sub AcceptSequence()
            SyncLock _SyncRoot
                If _SequenceGenerator IsNot Nothing Then
                    Try
                        _SequenceGenerator.Accept()
                    Finally
                        CancelSequence()
                    End Try
                End If
            End SyncLock
        End Sub
        Private Sub CancelSequence()
            SyncLock _SyncRoot
                UnSubscribeFromEvents()
                If _SequenceGenerator IsNot Nothing Then
                    _SequenceGenerator.Close()
                    _SequenceGenerator = Nothing
                End If
            End SyncLock
        End Sub
        Private Sub Session_AfterCommitTransaction(ByVal sender As Object, ByVal e As SessionManipulationEventArgs)
            AcceptSequence()
        End Sub
        Private Sub Session_AfterRollBack(ByVal sender As Object, ByVal e As SessionManipulationEventArgs)
            CancelSequence()
        End Sub
        Private Sub Session_FailedCommitTransaction(ByVal sender As Object, ByVal e As SessionOperationFailEventArgs)
            CancelSequence()
        End Sub
        Private Sub SubscribeToEvents()
            If Not (TypeOf Session Is NestedUnitOfWork) Then
                AddHandler Session.AfterCommitTransaction, AddressOf Session_AfterCommitTransaction
                AddHandler Session.AfterRollbackTransaction, AddressOf Session_AfterRollBack
                AddHandler Session.FailedCommitTransaction, AddressOf Session_FailedCommitTransaction
            End If
        End Sub
        Private Sub UnSubscribeFromEvents()
            If Not (TypeOf Session Is NestedUnitOfWork) Then
                RemoveHandler Session.AfterCommitTransaction, AddressOf Session_AfterCommitTransaction
                RemoveHandler Session.AfterRollbackTransaction, AddressOf Session_AfterRollBack
                RemoveHandler Session.FailedCommitTransaction, AddressOf Session_FailedCommitTransaction
            End If
        End Sub

#End Region

        '#Region "Sequence"



        '        Protected Overrides Sub OnSaving()
        '            Try
        '                MyBase.OnSaving()
        '                If Me.Session.IsNewObject(Me) AndAlso (Not GetType(NestedUnitOfWork).IsInstanceOfType(Session)) Then
        '                    Me.GetNextSequence(True)
        '                End If
        '            Catch
        '                Me.CancelSequence()
        '                Throw
        '            End Try
        '        End Sub

        '        Protected Overrides Sub OnDeleted()
        '            Try
        '                MyBase.OnDeleted()
        '                Me.DeleteSequence(True)
        '            Catch ex As Exception
        '                Me.CancelSequence()
        '                Throw
        '            End Try
        '        End Sub


        '        Public Function SetSequenceGenerator(subscribeToEvents As Boolean) As SequenceGenerator
        '            If _SequenceGenerator Is Nothing Then
        '                Dim typeToExistsMap As New Dictionary(Of String, Boolean)()
        '                For Each item As Object In Session.GetObjectsToSave()
        '                    typeToExistsMap(Session.GetClassInfo(item).FullName) = True
        '                Next item
        '                _SequenceGenerator = New SequenceGenerator(typeToExistsMap)
        '            End If
        '            If subscribeToEvents Then
        '                Me.SubscribeToEvents()
        '            End If
        '            Return _SequenceGenerator
        '        End Function

        '        Public Sub DeleteSequence(updateValue As Boolean)
        '            SyncLock _SyncRoot
        '                Me.SetSequenceGenerator(True)
        '                If _SequenceGenerator.DeleteSequence(Me, updateValue) Is Nothing OrElse Not updateValue Then
        '                    _SequenceGenerator.Dispose()
        '                    _SequenceGenerator = Nothing
        '                    Me.UnSubscribeFromEvents()
        '                End If
        '            End SyncLock
        '        End Sub

        '        Public Sub GetNextSequence(subscribeToEvents As Boolean)
        '            Me.GetNextSequence(subscribeToEvents, subscribeToEvents)
        '        End Sub

        '        Public Sub GetNextSequence(subscribeToEvents As Boolean, updateValue As Boolean)
        '            SyncLock _SyncRoot
        '                Me.SetSequenceGenerator(subscribeToEvents)
        '                _SequenceGenerator.GetNextSequence(Me, updateValue).SetSequenceValues()
        '                If Not updateValue Then
        '                    _SequenceGenerator.Dispose()
        '                    _SequenceGenerator = Nothing
        '                    Me.UnSubscribeFromEvents()
        '                End If
        '            End SyncLock
        '        End Sub
        '        Private Sub AcceptSequence()
        '            SyncLock _SyncRoot
        '                If _SequenceGenerator IsNot Nothing Then
        '                    Try
        '                        _SequenceGenerator.Accept()
        '                    Finally
        '                        CancelSequence()
        '                    End Try
        '                End If
        '            End SyncLock
        '        End Sub
        '        Private Sub CancelSequence()
        '            SyncLock _SyncRoot
        '                UnSubscribeFromEvents()
        '                If _SequenceGenerator IsNot Nothing Then
        '                    _SequenceGenerator.Close()
        '                    _SequenceGenerator = Nothing
        '                End If
        '            End SyncLock
        '        End Sub
        '        Private Sub Session_AfterCommitTransaction(ByVal sender As Object, ByVal e As SessionManipulationEventArgs)
        '            Me.AcceptSequence()
        '        End Sub
        '        Private Sub Session_AfterRollBack(ByVal sender As Object, ByVal e As SessionManipulationEventArgs)
        '            Me.CancelSequence()
        '        End Sub
        '        Private Sub Session_FailedCommitTransaction(ByVal sender As Object, ByVal e As SessionOperationFailEventArgs)
        '            Me.CancelSequence()
        '        End Sub
        '        Private Sub SubscribeToEvents()
        '            If Not (TypeOf Session Is NestedUnitOfWork) Then
        '                AddHandler Session.AfterCommitTransaction, AddressOf Session_AfterCommitTransaction
        '                AddHandler Session.AfterRollbackTransaction, AddressOf Session_AfterRollBack
        '                AddHandler Session.FailedCommitTransaction, AddressOf Session_FailedCommitTransaction
        '            End If
        '        End Sub
        '        Private Sub UnSubscribeFromEvents()
        '            If Not (TypeOf Session Is NestedUnitOfWork) Then
        '                RemoveHandler Session.AfterCommitTransaction, AddressOf Session_AfterCommitTransaction
        '                RemoveHandler Session.AfterRollbackTransaction, AddressOf Session_AfterRollBack
        '                RemoveHandler Session.FailedCommitTransaction, AddressOf Session_FailedCommitTransaction
        '            End If
        '        End Sub


        '#End Region

    End Class

End Namespace
