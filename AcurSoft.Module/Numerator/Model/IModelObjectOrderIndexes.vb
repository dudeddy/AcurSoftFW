
Imports AcurSoft.Numerator.Core
Imports AcurSoft.Numerator.Helpers
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.Model.Core

Namespace Numerator.Model


    <ModelNodesGenerator(GetType(ObjectOrderIndexesNodesGenerator))>
    Public Interface IModelObjectOrderIndexes
        Inherits IModelNode, IModelList(Of IModelObjectOrderIndex), IList(Of IModelObjectOrderIndex), ICollection(Of IModelObjectOrderIndex), IEnumerable(Of IModelObjectOrderIndex), IEnumerable
        Function GetModelObjectOrderIndex(type As Type) As IModelObjectOrderIndex
        Function GetModelObjectOrderIndexFromListViewId(viewId As String) As IModelObjectOrderIndex
    End Interface

    <DomainLogic(GetType(IModelObjectOrderIndexes))>
    Public Class ModelObjectOrderIndexesNodeLogic
        Public Shared Function GetModelObjectOrderIndex(ByVal instance As IModelObjectOrderIndexes, type As Type) As IModelObjectOrderIndex
            Return instance.OfType(Of IModelObjectOrderIndex).FirstOrDefault(Function(q) q.Valid AndAlso q.ModelClass.TypeInfo.Type Is type)
        End Function
        Public Shared Function GetModelObjectOrderIndexFromListViewId(ByVal instance As IModelObjectOrderIndexes, viewId As String) As IModelObjectOrderIndex
            Return instance.OfType(Of IModelObjectOrderIndex).FirstOrDefault(Function(q) q.Valid AndAlso q.ListView.Id = viewId)
        End Function

    End Class

    Public Class ObjectOrderIndexesNodesGenerator
        Inherits ModelNodesGeneratorBase
        Protected Overrides Sub GenerateNodesCore(ByVal node As ModelNode)

            Dim qry As List(Of ObjectNumeratorAttribute) = (
            From q In XafTypesInfo.Instance.PersistentTypes
            Where q.IsPersistent
            Select q, a = q.FindAttribute(Of ObjectNumeratorAttribute)()
            Where a IsNot Nothing
            Select a.Init(q.Type)).ToList()

            Dim i As Integer = 0
            For Each q In qry
                With node.AddNode(Of IModelObjectOrderIndex)(q.Id)
                    IndexNumeratorHelper.Assign(DirectCast(q, IIndexNumeratorObjectAttribute), DirectCast(.Self, IModelBaseOrderIndex))
                    '.ObjectName = q.ObjectName
                    '.IndexedBy = q.IndexedBy
                    '.IsEditable = q.IsEditable
                    '.DisplayFormat = q.DisplayFormat
                    '.EditMask = q.EditMask
                    '.AllowIndexEdit = q.AllowIndexEdit
                    '.IndexSortOrder = q.IndexSortOrder
                    '.NewIndexPosition = q.NewIndexPosition
                    .Index = i
                End With
                i += 1
            Next
        End Sub
    End Class
End Namespace
