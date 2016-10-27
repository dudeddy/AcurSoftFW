Imports System.ComponentModel
Imports System.Drawing.Design
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.Model.Core
Imports DevExpress.Persistent.Base

Namespace Numerator.Model

    Public Interface IModelOrderIndexes
        Inherits IModelNode ', IModelList(Of IModelListOrderIndex), IList(Of IModelListOrderIndex), ICollection(Of IModelListOrderIndex), IEnumerable(Of IModelListOrderIndex), IEnumerable
        ReadOnly Property ListOrderIndexes As IModelListOrderIndexes
        ReadOnly Property ObjectOrderIndexes As IModelObjectOrderIndexes

        'Function GetModelObjectOrderIndex(type As Type) As IModelObjectOrderIndex
        'Function GetModelListOrderIndex(type As Type, member As String) As IModelListOrderIndex
    End Interface

    '<DomainLogic(GetType(IModelOrderIndexes))>
    'Public Class ModelOrderIndexesNodeLogic
    '    Public Shared Function GetModelObjectOrderIndex(ByVal instance As IModelOrderIndexes, type As Type) As IModelObjectOrderIndex
    '        Return ModelObjectOrderIndexesNodeLogic.GetModelObjectOrderIndex(instance.ObjectOrderIndexes, type)
    '    End Function

    '    Public Shared Function GetModelListOrderIndex(ByVal instance As IModelOrderIndexes, type As Type, member As String) As IModelListOrderIndex
    '        Return ModelListOrderIndexesNodeLogic.GetModelListOrderIndex(instance.ListOrderIndexes, type, member)
    '    End Function


    'End Class
End Namespace
