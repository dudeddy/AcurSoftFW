Imports AcurSoft.UFId.Utils
Imports DevExpress.Data.Filtering
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Xpo

Namespace UFId.Utils
    Public Class UFIdHelper
        Public ReadOnly Property IsActive As Boolean
        Public ReadOnly Property Target As BaseObject

        'Public ReadOnly Property Links As XPCollection(Of UFId.DAL.UFIdConfigLink)

        Private _IsHandlingTransaction As Boolean
        Private Shared _SequenceGenerator As UFIdGenerator
        Private Shared _SyncRoot As New Object()
        'Public ReadOnly Property UseSequenceGenerator As Boolean?



        Public Sub New(o As BaseObject)
            Me.Target = o
            Me.IsActive = UFIdInitializer.ObjectsWithUFIds.Contains(o.ClassInfo.FullName)

        End Sub

        Public Function TrySave() As Boolean

            If Not (TypeOf Me.Target.Session Is NestedUnitOfWork) AndAlso (Me.Target.Session.DataLayer IsNot Nothing) AndAlso (TypeOf Me.Target.Session.ObjectLayer Is SimpleObjectLayer) Then
                'OR
                '&& !(Session.ObjectLayer is DevExpress.ExpressApp.Security.ClientServer.SecuredSessionObjectLayer)
                'Dim co As CriteriaOperator = CriteriaOperator.Parse("EditStatus.Status = ?", ObjectEditStatuses.Draft)
                GenerateSequence()
            End If
            Return True
            'Try
            'Catch ex As Exception
            '    Return False
            'End Try
        End Function



        Public Sub DeleteSequence()
            If Me.IsActive Then
                SyncLock _SyncRoot
                    'Me.SetSequenceGenerator(True)
                    Me.CreateSequenceGenerator()
                    'If Me.UseSequenceGenerator Then
                    'Me.SubscribeToEvents()

                    If _SequenceGenerator.SetSequenceAsDeleted(Me.Target).Count > 0 Then
                        _SequenceGenerator.Dispose()
                        _SequenceGenerator = Nothing
                        Me.UnSubscribeFromEvents()
                    End If

                    'End If
                End SyncLock
            End If
        End Sub


        Public Sub CreateSequenceGenerator()
            'Dim links As XPCollection(Of UFId.DAL.UFIdConfigLink) = Nothing
            If Me.IsActive Then

                'If Me.UseSequenceGenerator Is Nothing Then
                '    If UFIdGeneratorInitializer.ObjectsWithUFIds.Contains(Me.ClassInfo.FullName) Then
                '        _SequenceGenerator = New SequenceGenerator(Me)
                '        _UseSequenceGenerator = _SequenceGenerator.IsActive
                '    Else
                '        _UseSequenceGenerator = False
                '    End If
                'End If

                SyncLock _SyncRoot
                    If _SequenceGenerator Is Nothing Then
                        'Dim typeToExistsMap As Dictionary(Of String, Boolean) =
                        _SequenceGenerator = New UFIdGenerator(Me.GetTypeToExistsMap())
                    ElseIf Not _SequenceGenerator.IsInitiated
                        _SequenceGenerator.Init(Me.GetTypeToExistsMap())
                    End If
                End SyncLock
            End If
        End Sub

        Private Function GetTypeToExistsMap() As Dictionary(Of String, Boolean)
            Dim typeToExistsMap As New Dictionary(Of String, Boolean)()
            For Each item As Object In Me.Target.Session.GetObjectsToSave()
                typeToExistsMap(Me.Target.Session.GetClassInfo(item).FullName) = True
            Next item

            Return typeToExistsMap
        End Function

        Public Sub SetDraftSequenceValues()
            If Me.IsActive Then

                Me.CreateSequenceGenerator()
                SyncLock _SyncRoot
                    SubscribeToEvents()
                    'OnSequenceGenerated(_SequenceGenerator.GetNextSequence(Me))
                    _SequenceGenerator.SetDraftSequenceValues(Me.Target)
                End SyncLock
            End If
        End Sub

        Public Sub GenerateSequence()
            If Me.IsActive Then
                Me.CreateSequenceGenerator()
                SyncLock _SyncRoot
                    SubscribeToEvents()
                    'OnSequenceGenerated(_SequenceGenerator.GetNextSequence(Me))
                    _SequenceGenerator.SetNextSequence(Me.Target)
                End SyncLock
            End If
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
        Public Sub CancelSequence()
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
            If _IsHandlingTransaction Then Return
            If Not (TypeOf Me.Target.Session Is NestedUnitOfWork) Then

                AddHandler Me.Target.Session.AfterCommitTransaction, AddressOf Session_AfterCommitTransaction
                AddHandler Me.Target.Session.AfterRollbackTransaction, AddressOf Session_AfterRollBack
                AddHandler Me.Target.Session.FailedCommitTransaction, AddressOf Session_FailedCommitTransaction
                _IsHandlingTransaction = True
            End If
        End Sub
        Private Sub UnSubscribeFromEvents()
            If Not (TypeOf Me.Target.Session Is NestedUnitOfWork) Then
                RemoveHandler Me.Target.Session.AfterCommitTransaction, AddressOf Session_AfterCommitTransaction
                RemoveHandler Me.Target.Session.AfterRollbackTransaction, AddressOf Session_AfterRollBack
                RemoveHandler Me.Target.Session.FailedCommitTransaction, AddressOf Session_FailedCommitTransaction
                _IsHandlingTransaction = False
            End If
        End Sub
    End Class
End Namespace
