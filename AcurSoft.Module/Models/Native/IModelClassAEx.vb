Imports System.ComponentModel
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model

Namespace Model

    Public Interface IModelClassAEx
        Inherits IModelClass

        <Category("Behavior")>
        <ModelReadOnly(GetType(ModelClassAExPropertyReadOnlyCalculator))>
        Property CanCreateObjectTemplates As Boolean
    End Interface


    <DomainLogic(GetType(IModelClassAEx))>
    Public Class ModelClassAExNodeLogic

        Public Shared Function Get_CanCreateObjectTemplates(ByVal instance As IModelClassAEx) As Boolean
            Return instance.IsCreatableItem
        End Function

    End Class

    Public Class ModelClassAExPropertyReadOnlyCalculator
        Implements IModelIsReadOnly

        Public Function IsReadOnly(node As IModelNode, childNode As IModelNode) As Boolean Implements IModelIsReadOnly.IsReadOnly
            Return False
        End Function

        Public Function IsReadOnly(node As IModelNode, propertyName As String) As Boolean Implements IModelIsReadOnly.IsReadOnly
            Dim modelClass As IModelClassAEx = DirectCast(node, IModelClassAEx)
            Select Case propertyName
                Case "CanCreateObjectTemplates"
                    Return Not modelClass.IsCreatableItem
            End Select
            Return False
        End Function
    End Class


End Namespace
