Imports AcurSoft
Imports AcurSoft.Enums
Imports AcurSoft.Numerator.Model
Imports DevExpress.Data
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model
Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata
Imports AcurSoft.Data

'Imports System.Linq
Namespace Numerator.Core

    Public Class IndexBaseNumerator
        Implements IIndexNumerator
        Public Const IndexNumeratorName As String = "IndexNumerator"

        'Public Const CurrentIndexNumeratorHelperKey As String = "CurrentIndexNumeratorHelper"
        'Public Const CurrentFrame As String = "CurrentFrame"
        'Public Const ParentIndexNumeratorHelperKey As String = "ParentIndexNumeratorHelper"
        'Public Const ParentFrame As String = "ParentFrame"

        Public Overridable ReadOnly Property Model As IModelBaseOrderIndex

        Public Overridable ReadOnly Property TypeInfo As ITypeInfo
        Public Overridable ReadOnly Property IsActive As Boolean
        Public Overridable ReadOnly Property ObjectSpace As IObjectSpace
        Public ReadOnly Property IndexMember As IMemberInfo
        Public Overridable ReadOnly Property XpIndexMember As IMemberInfo
        Public ReadOnly Property IndexMemberName As String
        Public Overridable ReadOnly Property IndexType As Type


        Public ReadOnly Property LastIndex As Decimal?
            Get
                If Not IsActive Then Return Nothing
                If Me.Data.Count = 0 Then Return Nothing
                Return Me.Data.Max(Function(q) Me.GetIndex(q))
            End Get
        End Property
        Public ReadOnly Property FirstIndex As Decimal?
            Get
                If Not IsActive Then Return Nothing
                If Me.Data.Count = 0 Then Return Nothing
                Return Me.Data.Min(Function(q) Me.GetIndex(q))
            End Get
        End Property

        Public Overridable ReadOnly Property CriteriaUp As String
            Get
                If Not Me.IsActive Then Return Nothing
                Return Me.IndexMemberName & " > 1.0"
            End Get
        End Property
        Public Overridable ReadOnly Property CriteriaDown As String
            Get
                If Not Me.IsActive Then Return Nothing
                Return String.Format("{0} < [<{1}>].Max({0})", Me.IndexMemberName, Me.TypeInfo.Name)
            End Get
        End Property


        Public Function IsOrdered() As Boolean
            If Not Me.IsActive Then Return False
            Return Me.Data.Zip(Me.Data.Skip(1), Function(a, b) (New With {a, b})).All(Function(p) Convert.ToDecimal(Convert.ToInt32(Me.GetIndex(p.a)) = Me.GetIndex(p.a) AndAlso Me.GetIndex(p.a) = Me.GetIndex(p.b) - 1))
        End Function
        Public Overridable ReadOnly Property Data As IEnumerable(Of PersistentBase)
            Get
                If Not Me.IsActive Then Return Nothing
                'Return DirectCast(Me.ObjectSpace.Evaluate(Me.TypeInfo.Type, CriteriaOperator.Parse(String.Format("[<{0}>]", Me.TypeInfo.Name)), CriteriaOperator.Parse("1 = 1")), IList).OfType(Of PersistentBase)
                Return Me.ObjectSpace.CreateCollection(Me.TypeInfo.Type).OfType(Of PersistentBase)
            End Get
        End Property

        Public ReadOnly Property IndexSortOrder As ColumnSortOrder Implements IIndexNumerator.IndexSortOrder
        Public ReadOnly Property NewIndexPosition As NewItemPosition Implements IIndexNumerator.NewIndexPosition

        Public ReadOnly Property Incrementer As Integer



        'Public ReadOnly Property Details As IEnumerable(Of PersistentBase)
        '    Get
        '        If Not Me.IsActive Then Return Nothing
        '        Return DirectCast(Me.Master.ClassInfo.GetMember(Me.XafMasterMemberInfo.Name).GetValue(Me.Master), IList).OfType(Of PersistentBase)
        '    End Get
        'End Property

        'Public ReadOnly Property DataEx As IEnumerable(Of PersistentBase)
        '    Get
        '        If Not Me.IsActive Then Return Nothing
        '        'Return DirectCast(Me.ObjectSpace.Evaluate(Me.TypeInfo.Type, CriteriaOperator.Parse(String.Format("[<{0}>]", Me.TypeInfo.Name)), CriteriaOperator.Parse("1 = 1")), IList).OfType(Of PersistentBase)
        '        Return Me.ObjectSpace.CreateCollection(Me.TypeInfo.Type).OfType(Of PersistentBase)
        '    End Get
        'End Property

        'Private _ObjectSpaceGetter As Func(Of IObjectSpace)
        'Private _DataGetter As Func(Of IEnumerable(Of PersistentBase))
        Public Sub New()
        End Sub

        Public Overridable Sub SetXpMemberInfo()
            _XpIndexMember = Me.TypeInfo.FindMember(Me.IndexMemberName)
            If _XpIndexMember IsNot Nothing Then
                _IndexType = Me.IndexMember.MemberType

            End If
        End Sub

        Public Sub Init(model As IModelBaseOrderIndex)
            If model IsNot Nothing Then
                _Model = model
                _IsActive = model.Valid
                If Me.IsActive Then
                    _NewIndexPosition = model.NewIndexPosition
                    _IndexSortOrder = model.IndexSortOrder
                    If _IndexSortOrder = ColumnSortOrder.Descending Then
                        _Incrementer = -1
                    Else
                        _Incrementer = 1
                    End If
                    _TypeInfo = model.ModelClass.TypeInfo
                    _IndexMemberName = model.IndexedBy
                    _IndexMember = model.IndexMember
                    Me.SetXpMemberInfo()
                End If
            End If
        End Sub

        Public Sub New(model As IModelBaseOrderIndex)
            Me.Init(model)
        End Sub



        Public Sub New(model As IModelBaseOrderIndex, objectSpace As IObjectSpace)
            Me.ObjectSpace = objectSpace
            If model IsNot Nothing Then
                Me.Init(model)
            End If
        End Sub


        Public Sub ApplyOrder(orderData As IEnumerable(Of ColumnSortOrderInfo)) Implements IIndexNumerator.ApplyOrder
            Dim data As IEnumerable(Of PersistentBase) = Me.Data
            If data Is Nothing Then Return
            Dim detailsList As List(Of PersistentBase) = data.AsQueryable.OrderBy(orderData).ToList()

            Dim i As Decimal = 1

            For Each o In detailsList
                Me.SetIndex(o, i)
                i += 1
            Next
        End Sub

        Public Sub ApplyOrder(Optional checkOrder As Boolean = True) Implements IIndexNumerator.ApplyOrder
            If checkOrder Then
                If Me.IsOrdered Then Return
            End If
            'Dim lst As IEnumerable(Of PersistentBase) = Nothing
            'If Me.IndexSortOrder = ColumnSortOrder.Descending Then
            '    lst = Me.Data.OrderBy(Function(q) Me.IndexMember.GetValue(q))
            'Else

            'End If
            Dim lst As IEnumerable(Of PersistentBase) = Me.Data.OrderBy(Function(q) Me.IndexMember.GetValue(q))
            If lst Is Nothing Then Return
            Dim i As Decimal = 1

            For Each o In lst
                Me.SetIndex(o, i)
                i += 1
            Next
            'lst.FirstOrDefault.Session.
        End Sub

        Public Sub ApplyOrderEx()
            Dim lst As IEnumerable(Of PersistentBase) = Me.Data.OrderBy(Function(q) Me.IndexMember.GetValue(q))
            If lst Is Nothing Then Return
            Dim i As Decimal = 1

            For Each o In lst
                Me.SetIndex(o, i)
                i += 1
            Next
            'lst.FirstOrDefault.Session.
        End Sub


        Public Sub MoveToTop(ByVal movingObjects As IEnumerable(Of PersistentBase)) Implements IIndexNumerator.MoveToTop
            Dim count As Integer = movingObjects.Count
            If count = 0 Then Return
            Dim lst As List(Of PersistentBase) = movingObjects.OrderBy(Function(q) Me.GetIndex(q)).ToList
            Dim x As Integer = If(Me.IndexSortOrder = ColumnSortOrder.Descending, Me.LastIndex + 1000 + count, -1000 - count)
            For Each o In lst
                Me.SetIndex(o, x)
                x += 1
            Next
            Me.ApplyOrder(False)
        End Sub
        Public Sub MoveToTopCore(ByVal movingObject As PersistentBase)
            Me.SetIndex(movingObject, 0)
            Me.ApplyOrder(False)
        End Sub

        Public Sub MoveToTop(ByVal movingObject As PersistentBase) Implements IIndexNumerator.MoveToTop
            If Me.IndexSortOrder = ColumnSortOrder.Descending Then
                Me.MoveToLastCore(movingObject)
            Else
                Me.MoveToTopCore(movingObject)
            End If
            'Me.SetIndex(movingObject, 0)
            'Me.ApplyOrder(False)
        End Sub


        'Public Function SetNewObjectIndex(ByVal movingObject As PersistentBase, objectSpace As IObjectSpace) As Decimal ' Implements IIndexNumerator.SetNewObjectIndex
        '    Dim rtn As Decimal = Decimal.MinValue
        '    'If movingObject.Session.IsNewObject(movingObject) Then
        '    If objectSpace.IsNewObject(movingObject) Then
        '        Select Case Me.NewIndexPosition
        '            Case NewItemPosition.Bottom
        '                If Me.IndexSortOrder = ColumnSortOrder.Descending Then
        '                    rtn = -1
        '                    Me.SetIndex(movingObject, -1)
        '                Else
        '                    rtn = Me.SetToLast(movingObject)
        '                End If
        '            Case NewItemPosition.Top
        '                If Me.IndexSortOrder = ColumnSortOrder.Descending Then
        '                    rtn = Me.SetToLast(movingObject)
        '                Else
        '                    rtn = -1
        '                    Me.SetIndex(movingObject, -1)
        '                End If
        '        End Select
        '        'Else
        '        '    Dim x = 0
        '    End If
        '    Return rtn
        'End Function



        Public Function SetNewObjectIndex(ByVal movingObject As PersistentBase, Optional objectSpace As IObjectSpace = Nothing) As Decimal Implements IIndexNumerator.SetNewObjectIndex
            Dim rtn As Decimal = Decimal.MinValue
            'objectSpace = If(objectSpace, Me.ObjectSpace)
            'If movingObject.Session.IsNewObject(movingObject) Then
            'If Me.ObjectSpace.IsNewObject(movingObject) Then
            If If(objectSpace, Me.ObjectSpace).IsNewObject(movingObject) Then
                Select Case Me.NewIndexPosition
                    Case NewItemPosition.Bottom
                        If Me.IndexSortOrder = ColumnSortOrder.Descending Then
                            rtn = -1
                            Me.SetIndex(movingObject, -1)
                        Else
                            rtn = Me.SetToLast(movingObject)
                        End If
                    Case NewItemPosition.Top
                        If Me.IndexSortOrder = ColumnSortOrder.Descending Then
                            rtn = Me.SetToLast(movingObject)
                        Else
                            rtn = -1
                            Me.SetIndex(movingObject, -1)
                        End If
                End Select
                'Else
                '    Dim x = 0
            End If
            Return rtn
        End Function

        Public Sub SetToLastCore(ByVal movingObject As PersistentBase)
            Dim lastIndex As Decimal? = Me.LastIndex
            If Not lastIndex.HasValue Then Return
            Me.SetIndex(movingObject, lastIndex.Value + 1)
        End Sub


        Public Function SetToLast(ByVal movingObject As PersistentBase) As Decimal Implements IIndexNumerator.SetToLast
            Dim rtn As Decimal = 1
            Dim lastIndex As Decimal? = Me.LastIndex
            If lastIndex.HasValue Then
                rtn = lastIndex.Value + 1
            End If
            Me.SetIndex(movingObject, rtn)
            Return rtn
        End Function

        Public Sub MoveToLastCore(ByVal movingObject As PersistentBase)
            Me.SetToLastCore(movingObject)
            Me.ApplyOrder(False)
        End Sub


        Public Sub MoveToLast(ByVal movingObject As PersistentBase) Implements IIndexNumerator.MoveToLast
            If Me.IndexSortOrder = ColumnSortOrder.Descending Then
                Me.MoveToTopCore(movingObject)
            Else
                Me.MoveToLastCore(movingObject)
            End If

            'Me.SetToLast(movingObject)
            'Me.ApplyOrder(False)
        End Sub

        Public Sub MoveToLast(ByVal movingObjects As IEnumerable(Of PersistentBase)) Implements IIndexNumerator.MoveToLast
            Dim count As Integer = movingObjects.Count
            If count = 0 Then Return
            Dim lst As List(Of PersistentBase) = movingObjects.OrderBy(Function(q) Me.GetIndex(q)).ToList
            Dim x As Integer = If(Me.IndexSortOrder = ColumnSortOrder.Descending, -1000 - count, Me.LastIndex + 1000 + count)
            For Each o In lst
                Me.SetIndex(o, x)
                x += 1
            Next
            Me.ApplyOrder(False)
            'Dim count As Integer = movingObjects.Count
            'If count = 0 Then Return
            'Dim lst As List(Of PersistentBase) = movingObjects.OrderBy(Function(q) Me.GetIndex(q)).ToList
            'Dim x As Integer = Me.LastIndex + 1000
            'For Each o In lst
            '    Me.SetIndex(o, x)
            '    x += 1
            'Next
            'Me.ApplyOrder(False)
        End Sub


        Public Sub MoveUpCore(ByVal movingObject As PersistentBase)
            Dim firstIndex As Decimal? = Me.FirstIndex

            Dim i As Decimal = Me.GetIndex(movingObject)
            If Not firstIndex.HasValue OrElse i = firstIndex Then Return
            Dim newIndex As Decimal = i - 1
            Dim upperObject As PersistentBase = Me.Data.FirstOrDefault(Function(q) Me.GetIndex(q) = newIndex)
            Me.SetIndex(upperObject, i)
            Me.SetIndex(movingObject, newIndex)
            Me.ApplyOrder()
        End Sub

        Public Sub MoveUp(ByVal movingObject As PersistentBase) Implements IIndexNumerator.MoveUp
            If Me.IndexSortOrder = ColumnSortOrder.Descending Then
                Me.MoveDownCore(movingObject)
            Else
                Me.MoveUpCore(movingObject)
            End If

            'Dim firstIndex As Decimal? = Me.FirstIndex

            'Dim i As Decimal = Me.GetIndex(movingObject)
            'If Not firstIndex.HasValue OrElse i = firstIndex Then Return
            'Dim newIndex As Decimal = i - 1
            'Dim upperObject As PersistentBase = Me.Data.FirstOrDefault(Function(q) Me.GetIndex(q) = newIndex)
            'Me.SetIndex(upperObject, i)
            'Me.SetIndex(movingObject, newIndex)
            'Me.ApplyOrder()
        End Sub

        Public Sub MoveDownCore(ByVal movingObject As PersistentBase)
            Dim lastIndex As Decimal? = Me.LastIndex
            Dim i As Decimal = Me.GetIndex(movingObject)
            If Not lastIndex.HasValue OrElse lastIndex.Value = i Then Return
            Dim newIndex As Decimal = i + 1
            Dim lowerObject As Object = Me.Data.FirstOrDefault(Function(q) Me.GetIndex(q) = newIndex)
            Me.SetIndex(lowerObject, i)
            Me.SetIndex(movingObject, newIndex)
            Me.ApplyOrder()
        End Sub


        Public Sub MoveDown(ByVal movingObject As PersistentBase) Implements IIndexNumerator.MoveDown
            If Me.IndexSortOrder = ColumnSortOrder.Descending Then
                Me.MoveUpCore(movingObject)
            Else
                Me.MoveDownCore(movingObject)
            End If


            'Dim lastIndex As Decimal? = Me.LastIndex
            'Dim i As Decimal = Me.GetIndex(movingObject)
            'If Not lastIndex.HasValue OrElse lastIndex.Value = i Then Return
            'Dim newIndex As Decimal = i + 1
            'Dim lowerObject As Object = Me.Data.FirstOrDefault(Function(q) Me.GetIndex(q) = newIndex)
            'Me.SetIndex(lowerObject, i)
            'Me.SetIndex(movingObject, newIndex)
            'Me.ApplyOrder()
        End Sub

        ''' <summary> 
        ''' Remove. 
        ''' </summary>
        ''' <param name="movingObject">Object.</param>
        Public Sub Remove(ByVal movingObject As PersistentBase) Implements IIndexNumerator.Remove
            Dim i As Decimal = Me.GetIndex(movingObject)
            Dim toChange As List(Of PersistentBase) = Nothing
            If Me.IndexSortOrder = ColumnSortOrder.Descending Then
                toChange = Me.Data.Where(Function(q) Me.GetIndex(q) < i).ToList
            Else
                toChange = Me.Data.Where(Function(q) Me.GetIndex(q) > i).ToList
            End If

            For Each o In toChange
                Me.SetIndex(o, Me.GetIndex(o) - Me.Incrementer)
            Next
            'Dim i As Decimal = Me.GetIndex(movingObject)
            'Dim toChange As List(Of PersistentBase) = Me.Data.Where(Function(q) Me.GetIndex(q) > i).ToList
            'For Each o In toChange
            '    Me.SetIndex(o, Me.GetIndex(o) - 1)
            'Next
        End Sub

        Public Sub Remove(ByVal deletedObjects As IEnumerable(Of PersistentBase)) Implements IIndexNumerator.Remove
            Dim toChange As List(Of PersistentBase) = Me.Data.Except(deletedObjects).OrderBy(Function(q) Me.GetIndex(q)).ToList
            'DirectCast(Me.Details, IList).Remove(movingObject)
            Dim i As Decimal = 1

            For Each o In toChange
                Me.SetIndex(o, i)
                i += 1
            Next
            'Me.ApplyOrder()
        End Sub



        Public Function GetIndex(o As PersistentBase) As Decimal Implements IIndexNumerator.GetIndex
            Return Convert.ToDecimal(Me.XpIndexMember.GetValue(o))
        End Function

        Public Sub SetIndex(o As PersistentBase, i As Decimal) Implements IIndexNumerator.SetIndex
            Me.XpIndexMember.SetValue(o, Convert.ChangeType(i, Me.IndexType))
        End Sub

    End Class
End Namespace
