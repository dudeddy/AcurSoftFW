Imports DevExpress.ExpressApp
Imports DevExpress.Xpo
Imports AcurSoft.Data

Namespace Numerator.Core

    Public Interface IIndexNumerator
        Inherits IIndexCoreNumerator
        'ReadOnly Property IndexSortOrder As DevExpress.Data.ColumnSortOrder
        'ReadOnly Property NewIndexPosition As NewItemPosition

        Sub ApplyOrder(orderData As IEnumerable(Of ColumnSortOrderInfo))
        Sub ApplyOrder(Optional checkOrder As Boolean = True)
        Sub MoveToTop(ByVal movingObjects As IEnumerable(Of PersistentBase))
        Sub MoveToTop(ByVal movingObject As PersistentBase)
        Sub MoveToLast(ByVal movingObject As PersistentBase)
        Sub MoveToLast(ByVal movingObjects As IEnumerable(Of PersistentBase))
        Sub MoveUp(ByVal movingObject As PersistentBase)
        Sub MoveDown(ByVal movingObject As PersistentBase)
        Sub Remove(ByVal movingObject As PersistentBase)
        Sub Remove(ByVal deletedObjects As IEnumerable(Of PersistentBase))
        Function SetToLast(ByVal movingObject As PersistentBase) As Decimal
        Function SetNewObjectIndex(ByVal movingObject As PersistentBase, Optional objectSpace As IObjectSpace = Nothing) As Decimal

        Function GetIndex(o As PersistentBase) As Decimal
        Sub SetIndex(o As PersistentBase, i As Decimal)
    End Interface
End Namespace
