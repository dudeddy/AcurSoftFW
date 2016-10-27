Imports AcurSoft
Imports AcurSoft.Enums
Imports DevExpress.Data
Imports DevExpress.ExpressApp

Namespace Numerator.Core


    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=True)>
    Public Class ListNumeratorAttribute
        Inherits Attribute
        Implements IIndexNumeratorListAttribute

        Public ReadOnly Property MasterMember As String Implements IIndexNumeratorListAttribute.MasterMember
        Public ReadOnly Property ActiveCriteria As String Implements IIndexNumeratorListAttribute.ActiveCriteria
        Public ReadOnly Property Member As String Implements IIndexNumeratorListAttribute.Member


        Public ReadOnly Property IndexedBy As String Implements IIndexNumeratorListAttribute.IndexedBy
        Public ReadOnly Property Id As String Implements IIndexNumeratorObjectAttribute.Id
        Public ReadOnly Property IsEditable As Boolean Implements IIndexNumeratorObjectAttribute.IsEditable
        Public ReadOnly Property ObjectName As String Implements IIndexNumeratorObjectAttribute.ObjectName

        Public Property AllowIndexEdit As Boolean Implements IIndexNumeratorObjectAttribute.AllowIndexEdit
        Public Property EditMask As String = "N00" Implements IIndexNumeratorObjectAttribute.EditMask
        Public Property DisplayFormat As String = "{0:N00}" Implements IIndexNumeratorObjectAttribute.DisplayFormat
        Public Property IndexSortOrder As DevExpress.Data.ColumnSortOrder = DevExpress.Data.ColumnSortOrder.Ascending Implements IIndexNumeratorObjectAttribute.IndexSortOrder
        Public Property NewIndexPosition As NewItemPosition = NewItemPosition.Bottom Implements IIndexNumeratorObjectAttribute.NewIndexPosition


        Public Sub New(member As String, indexedBy As String, masterMember As String, Optional activeCriteria As String = Nothing, Optional isEditable As Boolean = False)
            Me.IndexedBy = indexedBy
            Me.Member = member
            Me.IsEditable = isEditable
            Me.MasterMember = masterMember
            If String.IsNullOrEmpty(activeCriteria) Then
                Me.ActiveCriteria = String.Format("[{0}] Is Not Null", masterMember)
            Else
                Me.ActiveCriteria = activeCriteria
            End If
        End Sub

        Public Function Init(type As Type) As ListNumeratorAttribute
            _ObjectName = XafTypesInfo.Instance.FindTypeInfo(type).Name
            _Id = Me.ObjectName & "-" & Me.Member & "-" & Me.IndexedBy
            Return Me
        End Function
    End Class
End Namespace
