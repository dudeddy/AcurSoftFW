'Imports AcurSoft.Helpers.Models
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Xpo

Namespace Helpers

    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=False)>
    Public Class PersistentTypeInfoAttribute
        Inherits Attribute
        Public ReadOnly Property TypeInfo As ITypeInfo

        Public Shared Function Setup(typeInfo As ITypeInfo) As PersistentTypeInfoAttribute
            Dim atrb As New PersistentTypeInfoAttribute(typeInfo)
            typeInfo.AddAttribute(atrb)
            Return atrb
        End Function


        Public Sub New(typeInfo As ITypeInfo)
            Me.TypeInfo = typeInfo
        End Sub

        Public ReadOnly Property DetailsOrderIndexes As Dictionary(Of String, String)
            Get
                Dim qry = From m In Me.TypeInfo.OwnMembers
                          Where m.IsList AndAlso m.IsAssociation  '.GetNode(m.Name).GetValue(Of String)("IndexedBy"))
                          Select m, n = ModuleCore.XApplication.FindModelClass(Me.TypeInfo.Type).OwnMembers.GetNode(m.Name)?.GetValue(Of String)("IndexedBy")
                          Where n IsNot Nothing

                Return qry.ToDictionary(Function(q) q.m.Name, Function(q) q.n)
            End Get
        End Property

        Public Function GetOrderIndex(master As ITypeInfo)
            Dim qry = From m In Me.TypeInfo.OwnMembers
                      Where m.IsAssociation AndAlso Not m.IsList  '.GetNode(m.Name).GetValue(Of String)("IndexedBy"))
                      Select m, a = m.FindAttribute(Of AssociationAttribute), o = m.AssociatedMemberInfo?.Owner
                      Where a IsNot Nothing 'AndAlso master.Equals(o)
            Return Nothing
        End Function

    End Class
End Namespace