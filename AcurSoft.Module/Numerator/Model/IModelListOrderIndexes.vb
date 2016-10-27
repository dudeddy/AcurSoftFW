
Imports AcurSoft.Numerator
Imports AcurSoft.Numerator.Core
Imports AcurSoft.Numerator.Helpers
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.Model.Core

Namespace Numerator.Model


    <ModelNodesGenerator(GetType(ListOrderIndexesNodesGenerator))>
    Public Interface IModelListOrderIndexes
        Inherits IModelNode, IModelList(Of IModelListOrderIndex), IList(Of IModelListOrderIndex), ICollection(Of IModelListOrderIndex), IEnumerable(Of IModelListOrderIndex), IEnumerable
        Function GetModelListOrderIndex(type As Type, member As String) As IModelListOrderIndex
        Function GetModelListOrderIndex(listViewId As String, member As String) As IModelListOrderIndex
        Function GetModelListOrderIndexesFromMasterType(type As Type) As IEnumerable(Of IModelListOrderIndex)
        Function GetModelListOrderIndexesFromDetailType(type As Type) As IEnumerable(Of IModelListOrderIndex)
        'Function GetModelListOrderIndexFromDetailType(type As Type, viewId As String) As IModelListOrderIndex
        Function GetModelListOrderIndexFromViewIds(masterViewId As String, detailViewId As String) As IModelListOrderIndex

        'Function GetModelListOrderIndexFromDetailVienId(ByVal instance As IModelListOrderIndexes, viewId As String) As IEnumerable(Of IModelListOrderIndex)
    End Interface

    <DomainLogic(GetType(IModelListOrderIndexes))>
    Public Class ModelListOrderIndexesNodeLogic
        Public Shared Function GetModelListOrderIndex(ByVal instance As IModelListOrderIndexes, type As Type, member As String) As IModelListOrderIndex
            Return instance.OfType(Of IModelListOrderIndex).FirstOrDefault(Function(q) q.Valid AndAlso q.ModelClass.TypeInfo.Type Is type AndAlso q.Member = member)
        End Function

        Public Shared Function GetModelListOrderIndex(ByVal instance As IModelListOrderIndexes, listViewId As String, member As String) As IModelListOrderIndex
            Return instance.OfType(Of IModelListOrderIndex).FirstOrDefault(Function(q) q.Valid AndAlso q.ListView.Id = listViewId AndAlso q.Member = member)
        End Function


        Public Shared Function GetModelListOrderIndexesFromMasterType(ByVal instance As IModelListOrderIndexes, type As Type) As IEnumerable(Of IModelListOrderIndex)
            Return instance.OfType(Of IModelListOrderIndex).Where(Function(q) q.Valid AndAlso q.ModelClass.TypeInfo.Type Is type)
        End Function
        Public Shared Function GetModelListOrderIndexesFromDetailType(ByVal instance As IModelListOrderIndexes, type As Type) As IEnumerable(Of IModelListOrderIndex)
            Return instance.OfType(Of IModelListOrderIndex).Where(Function(q) q.Valid AndAlso q.DetailModelClass.TypeInfo.Type Is type)
        End Function
        Public Shared Function GetModelListOrderIndexFromDetailVienId(ByVal instance As IModelListOrderIndexes, viewId As String) As IEnumerable(Of IModelListOrderIndex)
            Return instance.OfType(Of IModelListOrderIndex).Where(Function(q) q.Valid AndAlso q.MemberDetailView.Id = viewId)
        End Function
        Public Shared Function GetModelListOrderIndexFromViewIds(ByVal instance As IModelListOrderIndexes, masterViewId As String, detailViewId As String) As IModelListOrderIndex
            Return instance.OfType(Of IModelListOrderIndex).FirstOrDefault(Function(q) q.Valid AndAlso q.ListView.Id = masterViewId AndAlso q.MemberDetailView.Id = detailViewId)
        End Function

    End Class


    Public Class ListOrderIndexesNodesGenerator
        Inherits ModelNodesGeneratorBase
        Protected Overrides Sub GenerateNodesCore(ByVal node As ModelNode)

            Dim qry As List(Of ListNumeratorAttribute) =
            (From q In XafTypesInfo.Instance.PersistentTypes
             Where q.IsPersistent
             Select q, a = q.FindAttributes(Of ListNumeratorAttribute)()
             Where a.Count > 0
             From atr In a
             Select atr.Init(q.Type)).ToList()

            Dim i As Integer = 0
            For Each q In qry
                With node.AddNode(Of IModelListOrderIndex)(q.Id)
                    IndexNumeratorHelper.Assign(DirectCast(q, IIndexNumeratorListAttribute), DirectCast(.Self, IModelListOrderIndex))

                    .Index = i
                End With
                i += 1
            Next
        End Sub
    End Class
End Namespace
