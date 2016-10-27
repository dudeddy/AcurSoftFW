Imports AcurSoft
Imports AcurSoft.Enums
Imports AcurSoft.Model
Imports AcurSoft.Numerator.Core
Imports AcurSoft.Numerator.Model
Imports DevExpress.Data
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.Xpo
Imports AcurSoft.Data

Namespace Numerator.Helpers

    Public Class IndexNumeratorHelper
        Implements IIndexNumerator


        Public Shared Sub Assign(source As IIndexNumeratorObjectAttribute, target As IModelBaseOrderIndex)
            With target
                .ObjectName = source.ObjectName
                .IndexedBy = source.IndexedBy
                .IsEditable = source.IsEditable
                .DisplayFormat = source.DisplayFormat
                .EditMask = source.EditMask
                .AllowIndexEdit = source.AllowIndexEdit
                .IndexSortOrder = source.IndexSortOrder
                .NewIndexPosition = source.NewIndexPosition
            End With

        End Sub


        Public Shared Sub Assign(source As IIndexNumeratorListAttribute, target As IModelListOrderIndex)
            IndexNumeratorHelper.Assign(DirectCast(source, IIndexNumeratorObjectAttribute), DirectCast(target, IModelBaseOrderIndex))
            With target
                .ObjectName = source.ObjectName
                .Member = source.Member
                .ActiveCriteria = source.ActiveCriteria
            End With

        End Sub

        <Flags>
        Public Enum NumeratorKindEnum
            None
            ListIndex = 1 << 1
            ObjectIndex = 2 << 1
        End Enum

        Public Class ActionsInfosClass
            Public ReadOnly Property IsActive As Boolean
            Public ReadOnly Property TargetObjectType As Type
            Public ReadOnly Property IndexNumerator As IndexBaseNumerator
            Public ReadOnly Property CriteriaUp As String
            Public ReadOnly Property CriteriaDown As String

            Public Sub New(h As IndexNumeratorHelper)
                If h.IsActive AndAlso h.IsNested AndAlso h.NumeratorKind.HasFlag(IndexNumeratorHelper.NumeratorKindEnum.ListIndex) Then
                    Me.SetIndexNumerator(h.IndexListNumerator)
                    Me.TargetObjectType = h.IndexListNumerator.ModelEx.DetailModelClass.TypeInfo.Type

                ElseIf h.IsActive AndAlso Not h.IsNested AndAlso h.NumeratorKind.HasFlag(IndexNumeratorHelper.NumeratorKindEnum.ObjectIndex) Then
                    Me.SetIndexNumerator(h.IndexObjectNumerator)
                    Me.TargetObjectType = h.IndexObjectNumerator.TypeInfo.Type
                End If
            End Sub

            Public Sub SetIndexNumerator(indexNumerator As IndexBaseNumerator)
                _IndexNumerator = indexNumerator
                _IsActive = True
                If indexNumerator.IndexSortOrder = DevExpress.Data.ColumnSortOrder.Descending Then
                    _CriteriaUp = Me.IndexNumerator.CriteriaDown
                    _CriteriaDown = Me.IndexNumerator.CriteriaUp
                Else
                    _CriteriaUp = Me.IndexNumerator.CriteriaUp
                    _CriteriaDown = Me.IndexNumerator.CriteriaDown
                End If
            End Sub
        End Class

        Public Function GetNumeratorKind() As NumeratorKindEnum
            Dim kind As NumeratorKindEnum = NumeratorKindEnum.None
            If Me.IndexObjectNumerator IsNot Nothing AndAlso Me.IndexObjectNumerator.IsActive Then
                kind = kind Or NumeratorKindEnum.ObjectIndex
            End If
            If Me.IndexListNumerator IsNot Nothing AndAlso Me.IndexListNumerator.IsActive Then
                kind = kind Or NumeratorKindEnum.ListIndex
            End If
            Return kind

        End Function
        Public ReadOnly Property NewIndexPosition As NewItemPosition Implements IIndexNumerator.NewIndexPosition
        Public ReadOnly Property NumeratorKind As NumeratorKindEnum
        Public ReadOnly Property ActionsInfos As ActionsInfosClass
        '    Get
        '        Dim kind As NumeratorKindEnum = NumeratorKindEnum.None
        '        If Me.IndexObjectNumerator IsNot Nothing AndAlso Me.IndexObjectNumerator.IsActive Then
        '            kind = kind Or NumeratorKindEnum.ObjectIndex
        '        End If
        '        If Me.IndexListNumerator IsNot Nothing AndAlso Me.IndexListNumerator.IsActive Then
        '            kind = kind Or NumeratorKindEnum.ListIndex
        '        End If
        '        Return kind
        '    End Get
        'End Property

        Public ReadOnly Property ViewController As ViewController
        Public ReadOnly Property ParentHelper As IndexNumeratorHelper
        Public ReadOnly Property IsNested As Boolean
        Public ReadOnly Property IsListView As Boolean
        Public ReadOnly Property IsDetailView As Boolean
        'Public ReadOnly Property ObjectModel As IModelObjectOrderIndex
        'Public ReadOnly Property ListModel As IModelListOrderIndex
        Public ReadOnly Property IndexObjectNumerator As IndexObjectNumerator
        Public ReadOnly Property IndexListNumerator As IndexListNumerator
        Public ReadOnly Property Application As IModelApplicationAEx
        'Public ReadOnly Property DictionaryTag As Dictionary(Of String, Object)
        Public ReadOnly Property IndexNumerator As IndexBaseNumerator
        Public ReadOnly Property IndexSortOrder As DevExpress.Data.ColumnSortOrder Implements IIndexNumerator.IndexSortOrder
        Public ReadOnly Property Model As IModelBaseOrderIndex
        Public ReadOnly Property RequireAfterNewReorder As Boolean
        Public ReadOnly Property ObjectSpaceHandler As NumeratorObjectSpaceHandler

        Public ReadOnly Property IsActive As Boolean
        Public Function GetIndexNumerator() As IndexBaseNumerator
            Dim kind As NumeratorKindEnum = Me.NumeratorKind
            If kind.HasFlag(NumeratorKindEnum.ListIndex) Then
                Return Me.IndexListNumerator
            ElseIf kind.HasFlag(NumeratorKindEnum.ObjectIndex) Then
                Return Me.IndexObjectNumerator
            End If

            Return Nothing
        End Function

        Public Function GetIndexNumerator(kind As NumeratorKindEnum) As IndexBaseNumerator
            Select Case kind
                Case NumeratorKindEnum.ListIndex
                    Return Me.IndexListNumerator
                Case NumeratorKindEnum.ObjectIndex
                    Return IndexObjectNumerator
            End Select
            Return Nothing
        End Function

        Public Sub New(viewController As ViewController)
            Me.ViewController = viewController
            Me.Application = DirectCast(viewController.Application.Model, IModelApplicationAEx)

            Me.IsNested = TypeOf viewController.Frame Is NestedFrame

            Me.IsListView = TypeOf viewController.View.Model Is IModelListView
            If Not Me.IsListView Then
                Me.IsDetailView = TypeOf viewController.View.Model Is IModelDetailView
            End If
            '_DictionaryTag = viewController.View.GetDictionaryTag()

            'Dim linkToListViewController As LinkToListViewController = viewController.Frame.GetController(Of LinkToListViewController)
            'Dim detailViewLinkController As DetailViewLinkController = viewController.Frame.GetController(Of DetailViewLinkController)

            'Dim xx = linkToListViewController.Link
            'Dim pp = detailViewLinkController.View


            If Me.IsListView Then
                'Me.IsListView = True
                Dim listView As ListView = DirectCast(viewController.View, ListView)
                'Dim model As IModelListView = DirectCast(viewController.View.Model, IModelListView)
                Me.IndexObjectNumerator = New IndexObjectNumerator(listView)
                If Me.IsNested Then
                    'Me.IndexListNumerator = New IndexListNumerator(DirectCast(viewController.View, ListView))
                    Dim model As IModelListOrderIndex = Me.Application.OrderIndexes.ListOrderIndexes.GetModelListOrderIndex(viewController.View.Id, DirectCast(viewController.Frame, NestedFrame).ViewItem.Id)
                    Me.IndexListNumerator = New IndexListNumerator(model, listView)
                Else
                    Dim listOrderIndexex As IEnumerable(Of IModelListOrderIndex) = Me.Application.OrderIndexes.ListOrderIndexes.GetModelListOrderIndexesFromMasterType(viewController.View.ObjectTypeInfo.Type)
                    Dim oo = 0
                End If
                'ElseIf TypeOf viewController.View.Model Is IModelDetailView

                '    Me.IsDetailView = True
                '    If _DictionaryTag.ContainsKey(IndexBaseNumerator.ParentFrame) Then
                '        Dim parentFrame As Frame = DirectCast(_DictionaryTag(IndexBaseNumerator.ParentFrame), Frame)
                '        If TypeOf parentFrame.View.Model Is IModelListView Then
                '            Dim modelList As IModelListOrderIndex = Me.Application.OrderIndexes.ListOrderIndexes.GetModelListOrderIndexFromViewIds(parentFrame.View.Id, viewController.View.Id)
                '            If modelList Is Nothing Then
                '                'Dim modelObject As IModelObjectOrderIndex = Me.Application.OrderIndexes.ObjectOrderIndexes.GetModelObjectOrderIndexFromViewId(parentFrame.View.Id)
                '                Me.IndexObjectNumerator = New IndexObjectNumerator(DirectCast(parentFrame.View, ListView))
                '            Else
                '                Me.IndexListNumerator = New IndexListNumerator(modelList, DirectCast(parentFrame.View, ListView))
                '            End If
                '        End If
                '        Dim x = 0

                '    End If
                'ElseIf Me.IsDetailView AndAlso Me.GetNumeratorKind() = NumeratorKindEnum.None Then
            ElseIf Me.IsDetailView Then
                'Dim detailView As DetailView = DirectCast(viewController.View, DetailView)
                Dim modelObject As IModelObjectOrderIndex = Me.Application.OrderIndexes.ObjectOrderIndexes.GetModelObjectOrderIndex(viewController.View.ObjectTypeInfo.Type)
                If modelObject Is Nothing Then
                    Dim modelLists As IEnumerable(Of IModelListOrderIndex) = Me.Application.OrderIndexes.ListOrderIndexes.GetModelListOrderIndexesFromDetailType(viewController.View.ObjectTypeInfo.Type)
                    Dim o As PersistentBase = DirectCast(Me.ViewController.View.CurrentObject, PersistentBase)
                    For Each q In modelLists
                        If q.ValidActiveCriteriaOperator.HasValue Then
                            Dim x = viewController.View.ObjectSpace.IsObjectFitForCriteria(Me.ViewController.View.CurrentObject, q.ValidActiveCriteriaOperator.Value)
                            Dim pppp = 3
                        End If
                    Next
                    Dim modelList As IModelListOrderIndex = modelLists.FirstOrDefault(Function(q)
                                                                                          With q.ValidActiveCriteriaOperator
                                                                                              If Not .HasValue Then Return False
                                                                                              Return viewController.View.ObjectSpace.IsObjectFitForCriteria(Me.ViewController.View.CurrentObject, .Value)
                                                                                          End With
                                                                                      End Function)


                    If modelList IsNot Nothing Then
                        Me.IndexListNumerator = New IndexListNumerator(modelList, DirectCast(Me.ViewController.View, DetailView))
                    End If


                Else
                    Me.IndexObjectNumerator = New IndexObjectNumerator(modelObject, DirectCast(Me.ViewController.View, DetailView))
                End If
            End If
            Me.NumeratorKind = Me.GetNumeratorKind()
            Me.IndexNumerator = Me.GetIndexNumerator()
            Me.IsActive = Me.NumeratorKind <> NumeratorKindEnum.None AndAlso Me.IndexNumerator IsNot Nothing

            Me.ActionsInfos = New ActionsInfosClass(Me)
            Me.IndexSortOrder = DevExpress.Data.ColumnSortOrder.None
            If Me.IsActive Then
                Me.Model = Me.IndexNumerator.Model
                Me.IndexSortOrder = Me.Model.IndexSortOrder
                Me.NewIndexPosition = Me.Model.NewIndexPosition
                With Me.Model
                    If (.NewIndexPosition = NewItemPosition.Top AndAlso .IndexSortOrder <> ColumnSortOrder.Descending) OrElse (.NewIndexPosition = NewItemPosition.Bottom AndAlso .IndexSortOrder <> ColumnSortOrder.Ascending) Then
                        Me.RequireAfterNewReorder = True
                    End If
                End With
                'If Me.NumeratorKind = NumeratorKindEnum.ListIndex Then

                'End If
                Me.ObjectSpaceHandler = New NumeratorObjectSpaceHandler(Me)

                'If Me.IsListView Then
                '    Me.ObjectSpaceHandler = New NumeratorObjectSpaceHandler(Me)
                'End If
            End If
        End Sub

        Public Sub CommitChanges()
            If Not Me.IsNested Then
                Me.IndexNumerator.ObjectSpace.CommitChanges()
            End If
        End Sub

        Public Sub ApplyOrder(orderData As IEnumerable(Of ColumnSortOrderInfo)) Implements IIndexNumerator.ApplyOrder
            If Not Me.IsActive Then Return
            Me.IndexNumerator.ApplyOrder(orderData)
            Me.CommitChanges()
        End Sub

        Public Sub ApplyOrder(Optional checkOrder As Boolean = True) Implements IIndexNumerator.ApplyOrder
            If Not Me.IsActive Then Return
            Me.IndexNumerator.ApplyOrder(checkOrder)
            Me.CommitChanges()
        End Sub

        Public Sub MoveToTop(movingObjects As IEnumerable(Of PersistentBase)) Implements IIndexNumerator.MoveToTop
            If Not Me.IsActive Then Return
            Me.IndexNumerator.MoveToTop(movingObjects)
            Me.CommitChanges()
        End Sub

        Public Sub MoveToTop(movingObject As PersistentBase) Implements IIndexNumerator.MoveToTop
            If Not Me.IsActive Then Return
            Me.IndexNumerator.MoveToTop(movingObject)
            Me.CommitChanges()
        End Sub

        Public Sub MoveToLast(movingObject As PersistentBase) Implements IIndexNumerator.MoveToLast
            If Not Me.IsActive Then Return
            Me.IndexNumerator.MoveToLast(movingObject)
            Me.CommitChanges()
        End Sub

        Public Sub MoveToLast(movingObjects As IEnumerable(Of PersistentBase)) Implements IIndexNumerator.MoveToLast
            If Not Me.IsActive Then Return
            Me.IndexNumerator.MoveToLast(movingObjects)
            Me.CommitChanges()
        End Sub

        Public Sub MoveUp(movingObject As PersistentBase) Implements IIndexNumerator.MoveUp
            If Not Me.IsActive Then Return
            Me.IndexNumerator.MoveUp(movingObject)
            Me.CommitChanges()
        End Sub

        Public Sub MoveDown(movingObject As PersistentBase) Implements IIndexNumerator.MoveDown
            If Not Me.IsActive Then Return
            Me.IndexNumerator.MoveDown(movingObject)
            Me.CommitChanges()
        End Sub

        Public Sub Remove(movingObject As PersistentBase) Implements IIndexNumerator.Remove
            If Not Me.IsActive Then Return
            Me.IndexNumerator.Remove(movingObject)
            'Me.IndexNumerator.ObjectSpace.CommitChanges()
        End Sub

        Public Sub Remove(deletedObjects As IEnumerable(Of PersistentBase)) Implements IIndexNumerator.Remove
            If Not Me.IsActive Then Return
            Me.IndexNumerator.Remove(deletedObjects)
            'Me.CommitChanges()
        End Sub

        Public Function GetIndex(o As PersistentBase) As Decimal Implements IIndexNumerator.GetIndex
            If Not Me.IsActive Then Return -1
            Me.IndexNumerator.GetIndex(o)
        End Function

        Public Sub SetIndex(o As PersistentBase, i As Decimal) Implements IIndexNumerator.SetIndex
            If Not Me.IsActive Then Return
            Me.IndexNumerator.SetIndex(o, i)
        End Sub

        Public Function SetToLast(o As PersistentBase) As Decimal Implements IIndexNumerator.SetToLast
            If Not Me.IsActive Then Return Decimal.MinValue
            Return Me.IndexNumerator.SetToLast(o)
            'Me.IndexNumerator.ObjectSpace.CommitChanges()
        End Function

        Public Function SetNewObjectIndex(o As PersistentBase, Optional objectSpace As IObjectSpace = Nothing) As Decimal Implements IIndexNumerator.SetNewObjectIndex
            If Not Me.IsActive Then Return Decimal.MinValue
            Return Me.IndexNumerator.SetNewObjectIndex(o, objectSpace)
            'Me.IndexNumerator.ObjectSpace.CommitChanges()
        End Function
    End Class
End Namespace
