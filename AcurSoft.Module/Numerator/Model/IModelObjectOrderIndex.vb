Imports System.ComponentModel
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model
Imports DevExpress.Persistent.Base

Namespace Numerator.Model


    <KeyProperty("Id")>
    Public Interface IModelObjectOrderIndex
        Inherits IModelNode, IModelBaseOrderIndex
        '<Category("Data")>
        '<DataSourceProperty("ModelClassDataSource")>
        'Property ModelClass As IModelClass

    End Interface

    <DomainLogic(GetType(IModelObjectOrderIndex))>
    Public Class ModelObjectOrderIndexNodeLogic


        Public Shared Function Get_IndexMember(ByVal instance As IModelObjectOrderIndex) As IMemberInfo
            If Not instance.Valid Then Return Nothing
            Return instance.ModelClass.FindOwnMember(instance.IndexedBy).MemberInfo
        End Function


        Public Shared Function Get_ListView(ByVal instance As IModelObjectOrderIndex) As IModelListView
            If Not instance.Valid Then Return Nothing
            'Master_Details_ListView
            Dim id As String = instance.ModelClass.TypeInfo.Name & "_ListView"
            Return DirectCast(instance.Application.Views.GetNode(id), IModelListView)

        End Function


        Public Shared Function Get_IndexedByDataSource(ByVal instance As IModelObjectOrderIndex) As IEnumerable(Of String)
            If instance.ModelClass Is Nothing Then Return New List(Of String)

            Dim qry = From q In instance.ModelClass.OwnMembers
                      Where q.MemberInfo.MemberType.IsNumericType()
                      Select q.Name

            Dim old = From q In DirectCast(instance.Parent, IModelObjectOrderIndexes).OfType(Of IModelObjectOrderIndex)
                      Where q.ModelClass.Name = instance.ModelClass.Name AndAlso q.IndexedBy = instance.IndexedBy AndAlso q.Id <> instance.Id
                      Select q.IndexedBy

            Return qry.Except(old).ToList()
        End Function

        Public Shared Function Get_Valid(ByVal instance As IModelObjectOrderIndex) As Boolean
            If String.IsNullOrEmpty(instance.IndexedBy) Then Return False
            If instance.ModelClass Is Nothing Then Return False
            If Not instance.ModelClass.OwnMembers.Any(Function(q) q.Name = instance.IndexedBy AndAlso q.MemberInfo.MemberType.IsNumericType()) Then Return False
            Return True
        End Function

    End Class

End Namespace
