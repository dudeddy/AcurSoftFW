Imports AcurSoft.Enums
Imports DevExpress.ExpressApp
Namespace Numerator.Core


    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=False)>
    Public Class ObjectNumeratorAttribute
        Inherits Attribute
        Implements IIndexNumeratorObjectAttribute
        Public ReadOnly Property IndexedBy As String Implements IIndexNumeratorObjectAttribute.IndexedBy
        Public ReadOnly Property Id As String Implements IIndexNumeratorObjectAttribute.Id
        Public ReadOnly Property IsEditable As Boolean Implements IIndexNumeratorObjectAttribute.IsEditable
        Public ReadOnly Property ObjectName As String Implements IIndexNumeratorObjectAttribute.ObjectName

        Public Property AllowIndexEdit As Boolean Implements IIndexNumeratorObjectAttribute.AllowIndexEdit
        Public Property EditMask As String = "N00" Implements IIndexNumeratorObjectAttribute.EditMask
        Public Property DisplayFormat As String = "{0:N00}" Implements IIndexNumeratorObjectAttribute.DisplayFormat
        Public Property IndexSortOrder As DevExpress.Data.ColumnSortOrder = DevExpress.Data.ColumnSortOrder.Ascending Implements IIndexNumeratorObjectAttribute.IndexSortOrder
        Public Property NewIndexPosition As NewItemPosition = NewItemPosition.Bottom Implements IIndexNumeratorObjectAttribute.NewIndexPosition

        Public Sub New(indexedBy As String, Optional isEditable As Boolean = False)
            Me.IndexedBy = indexedBy
            Me.IsEditable = isEditable
        End Sub

        Public Function Init(type As Type) As ObjectNumeratorAttribute
            _ObjectName = XafTypesInfo.Instance.FindTypeInfo(type).Name
            _Id = Me.ObjectName & "-" & Me.IndexedBy
            Return Me
        End Function
    End Class
End Namespace
