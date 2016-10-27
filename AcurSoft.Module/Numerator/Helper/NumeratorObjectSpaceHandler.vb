Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Actions
Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.Xpo
Namespace Numerator.Helpers

    Public Class NumeratorObjectSpaceHandler
        Public ReadOnly Property Helper As IndexNumeratorHelper
        Public ReadOnly Property NewObjectViewController As NewObjectViewController
        Public ReadOnly Property DeleteObjectsViewController As DeleteObjectsViewController

        Public Sub New(indexNumeratorHelper As IndexNumeratorHelper)
            Me.Helper = indexNumeratorHelper

            AddHandler indexNumeratorHelper.ViewController.ViewControlsCreated, AddressOf ViewController_ViewControlsCreated
            AddHandler indexNumeratorHelper.ViewController.Deactivated, AddressOf ViewController_Deactivated


        End Sub

        Public Sub ReorderObjectsAfterNew(objectSpace As IObjectSpace)
            Dim lst As IEnumerable(Of PersistentBase) = objectSpace.CreateCollection(Me.Helper.IndexNumerator.TypeInfo.Type).OfType(Of PersistentBase).OrderBy(Function(q) Me.Helper.IndexNumerator.IndexMember.GetValue(q)) '.Where(Function(q) Not Me.ObjectSpace.IsNewObject(q))
            'If listView Is Nothing Then
            '    Me.Helper.IndexNumerator.ApplyOrder(False)
            '    AddHandler objectSpace.Committed, AddressOf ObjectSpace_Committed
            '    objectSpace.CommitChanges()
            '    Return
            'End If


            If lst Is Nothing Then Return
            Dim i As Decimal = 1
            For Each o In lst
                Me.Helper.SetIndex(o, i)
                Dim n = objectSpace.GetObject(o)
                Me.Helper.SetIndex(n, i)
                i += 1
            Next
            Dim linkToListViewController As LinkToListViewController = Me.Helper.ViewController.Frame?.GetController(Of LinkToListViewController)
            Dim listView As ListView = linkToListViewController?.Link?.ListView
            If listView IsNot Nothing Then
                Dim listViewObjectSpace As IObjectSpace = listView.ObjectSpace
                AddHandler listViewObjectSpace.Committed, AddressOf ObjectSpace_Committed
                listViewObjectSpace.CommitChanges()

            End If
            AddHandler objectSpace.Committed, AddressOf ObjectSpace_Committed
            objectSpace.CommitChanges()
        End Sub


        Public Sub ReorderDetailsAfterNew(objectSpace As IObjectSpace)
            'Dim isList As Boolean = Me.Helper.IndexNumerator.Model.IsList
            Dim lst As IEnumerable(Of PersistentBase) = Nothing
            Dim os As IObjectSpace = objectSpace
            'Dim lstFound As Boolean = False
            If Me.Helper.IsListView Then
                lst = Me.Helper.IndexNumerator.Data
            ElseIf Me.Helper.IsDetailView
                Dim linkToListViewController As LinkToListViewController = Me.Helper.ViewController.Frame?.GetController(Of LinkToListViewController)
                Dim listView As ListView = linkToListViewController?.Link?.ListView
                If listView Is Nothing Then
                    Me.Helper.IndexNumerator.ApplyOrder(False)
                    Return
                End If
                Dim listViewObjectSpace As IObjectSpace = listView.ObjectSpace
                lst = listView.CollectionSource.List.OfType(Of PersistentBase) '.OrderBy(Function(q) Me.IndexNumerator.IndexMember.GetValue(q))
                os = listViewObjectSpace
            End If
            lst = lst.OrderBy(Function(q) Me.Helper.IndexNumerator.IndexMember.GetValue(q))
            Dim i As Decimal = 1
            For Each o In lst
                Dim oldIndex As Decimal = Me.Helper.IndexNumerator.IndexMember.GetValue(o)
                If oldIndex = -1 Then
                    '_ObjectToReload = o
                End If
                Me.Helper.SetIndex(o, i)
                i += 1
            Next
        End Sub


        Private Sub ViewController_Deactivated(sender As Object, e As EventArgs)
            RemoveHandler Me.Helper.ViewController.ViewControlsCreated, AddressOf ViewController_ViewControlsCreated
            RemoveHandler Me.Helper.ViewController.Deactivated, AddressOf ViewController_Deactivated

            If Me.NewObjectViewController IsNot Nothing Then
                RemoveHandler Me.NewObjectViewController.ObjectCreated, AddressOf NewObjectViewController_ObjectCreated
            End If

            If Me.Helper.IsListView Then
                If _DeleteObjectsViewController IsNot Nothing Then
                    RemoveHandler _DeleteObjectsViewController.DeleteAction.ExecuteCompleted, AddressOf IndexNumeratorDeleteActionExecuteCompleted
                End If
            End If


        End Sub

        Private Sub ViewController_ViewControlsCreated(sender As Object, e As EventArgs)
            RemoveHandler Helper.ViewController.ViewControlsCreated, AddressOf ViewController_ViewControlsCreated

            _NewObjectViewController = Helper.ViewController.Frame.GetController(Of DevExpress.ExpressApp.SystemModule.NewObjectViewController)
            AddHandler Me.NewObjectViewController.ObjectCreated, AddressOf NewObjectViewController_ObjectCreated

            If Me.Helper.IsListView Then
                _DeleteObjectsViewController = Helper.ViewController.Frame.GetController(Of DeleteObjectsViewController)
                AddHandler _DeleteObjectsViewController.DeleteAction.ExecuteCompleted, AddressOf IndexNumeratorDeleteActionExecuteCompleted
            End If

        End Sub

        Private Sub IndexNumeratorDeleteActionExecuteCompleted(sender As Object, e As ActionBaseEventArgs)
            Me.Helper.ApplyOrder(False)
        End Sub

        Private Sub NewObjectViewController_ObjectCreated(sender As Object, e As ObjectCreatedEventArgs)
            Dim newIndex As Decimal = Me.Helper.SetNewObjectIndex(e.CreatedObject, e.ObjectSpace)
            If Me.Helper.RequireAfterNewReorder Then
                If Me.Helper.Model.IsList Then
                    Me.ReorderDetailsAfterNew(e.ObjectSpace)
                End If
                AddHandler e.ObjectSpace.ObjectSaved, AddressOf ObjectSpace_ObjectSaved
            End If

        End Sub

        Private Sub ObjectSpace_ObjectSaved(sender As Object, e As ObjectManipulatingEventArgs)
            Dim objectSpace As IObjectSpace = DirectCast(sender, IObjectSpace)
            RemoveHandler objectSpace.ObjectSaved, AddressOf ObjectSpace_ObjectSaved
            Dim newIndex As Decimal = Me.Helper.SetNewObjectIndex(e.Object, objectSpace)
            If Me.Helper.RequireAfterNewReorder Then
                If Me.Helper.Model.IsList Then
                    Me.ReorderDetailsAfterNew(objectSpace)
                Else
                    Me.ReorderObjectsAfterNew(objectSpace)
                End If
                AddHandler objectSpace.Committed, AddressOf ObjectSpace_Committed
            End If

        End Sub


        Private Sub ObjectSpace_Committed(sender As Object, e As EventArgs)
            Dim objectSpace As IObjectSpace = DirectCast(sender, IObjectSpace)
            RemoveHandler objectSpace.Committed, AddressOf ObjectSpace_Committed
            If Me.Helper.RequireAfterNewReorder Then
                If Me.Helper.Model.IsList Then
                    objectSpace.Refresh()
                End If
            End If
        End Sub

    End Class
End Namespace
