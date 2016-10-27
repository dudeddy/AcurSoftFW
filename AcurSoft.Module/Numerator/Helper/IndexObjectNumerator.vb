Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Model
Imports DevExpress.Xpo
Imports AcurSoft.Model
Imports AcurSoft.Numerator.Model
Imports AcurSoft.Numerator.Core

Namespace Numerator


    Public Class IndexObjectNumerator
        Inherits IndexBaseNumerator

        Public Overrides ReadOnly Property ObjectSpace As IObjectSpace

        Public Overrides ReadOnly Property Data As IEnumerable(Of PersistentBase)
            Get
                If Not Me.IsActive Then Return Nothing
                'Return DirectCast(Me.ObjectSpace.Evaluate(Me.TypeInfo.Type, CriteriaOperator.Parse(String.Format("[<{0}>]", Me.TypeInfo.Name)), CriteriaOperator.Parse("1 = 1")), IList).OfType(Of PersistentBase)
                If Me.ListView Is Nothing Then
                    'If Me.DetailView IsNot Nothing Then

                    '    Dim master As Object = Me.MasterMember.GetValue(Me.DetailView.CurrentObject)
                    '    Return DirectCast(Me.ListMember.GetValue(master), IList).OfType(Of PersistentBase).Where(Function(q) Not Me.ObjectSpace.IsNewObject(q))

                    'End If

                    Return Me.ObjectSpace.CreateCollection(Me.TypeInfo.Type).OfType(Of PersistentBase).Where(Function(q) Not Me.ObjectSpace.IsNewObject(q))
                End If
                Return Me.ListView.CollectionSource.List.OfType(Of PersistentBase)
            End Get
        End Property

        Public ReadOnly Property ModelEx As IModelObjectOrderIndex
        Public ReadOnly Property ListView As ListView
        Public ReadOnly Property DetailView As DetailView

        Public Sub New(listView As ListView)
            Me.ModelEx = DirectCast(listView.Model.Application, IModelApplicationAEx).OrderIndexes.ObjectOrderIndexes.GetModelObjectOrderIndex(listView.Model.ModelClass.TypeInfo.Type)
            Me.ListView = listView
            Me.ObjectSpace = listView.ObjectSpace
            Me.Init(Me.ModelEx)
        End Sub

        Private Sub New(model As IModelObjectOrderIndex, objectSpace As IObjectSpace)
            MyBase.New(model, objectSpace)
            Me.ModelEx = model
            Me.ObjectSpace = objectSpace
        End Sub
        Public Sub New(model As IModelObjectOrderIndex, detailView As DetailView)
            Me.New(model, detailView.ObjectSpace)
            Me.DetailView = detailView
        End Sub
    End Class
End Namespace
