Imports System.ComponentModel
Imports AcurSoft.Core
Imports DevExpress.Data
Imports DevExpress.Xpo

Namespace Data


    Public Class ColumnSortOrderInfo
        Public ReadOnly Property Index As Integer
        Public ReadOnly Property ColumnSortOrder As ColumnSortOrder
        Public ReadOnly Property FieldName As String
        Public Property Caption As String

        Public Sub New(index As Integer, fieldName As String, columnSortOrder As ColumnSortOrder)
            Me.Index = index
            Me.FieldName = fieldName
            Me.ColumnSortOrder = columnSortOrder
        End Sub

        Public Sub New(index As Integer, fieldName As String, listSortOrder As ListSortDirection)
            Me.Index = index
            Me.FieldName = fieldName
            Me.ColumnSortOrder = If(listSortOrder = ListSortDirection.Ascending, ColumnSortOrder.Ascending, ColumnSortOrder.Descending)
        End Sub

        Public Sub New(index As Integer, fieldName As String, descending As Boolean)
            Me.Index = index
            Me.FieldName = fieldName
            Me.ColumnSortOrder = If(descending, ColumnSortOrder.Descending, ColumnSortOrder.Ascending)
        End Sub

        Public Sub New(index As Integer, fieldName As String)
            Me.Index = index
            Me.FieldName = fieldName
            Me.ColumnSortOrder = ColumnSortOrder.Ascending
        End Sub

        Public Function IsValid(o As PersistentBase) As XNullable(Of Metadata.XPMemberInfo)
            Return IsValid(o.ClassInfo)
        End Function
        Public Function IsValid(ci As Metadata.XPClassInfo) As XNullable(Of Metadata.XPMemberInfo)
            Return New XNullable(Of Metadata.XPMemberInfo)(ci.GetMember(Me.FieldName))
        End Function
    End Class
End Namespace
