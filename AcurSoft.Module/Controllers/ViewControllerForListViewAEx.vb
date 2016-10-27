'Imports AcurSoft.Helpers.Models
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Actions

Imports DevExpress.ExpressApp.Editors

Imports DevExpress.ExpressApp.Model

Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.Xpo

Imports Microsoft.VisualBasic

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DevExpress.Persistent.Base
Imports AcurSoft.Numerator.Helpers
Imports AcurSoft.Numerator
Imports AcurSoft.Numerator.Core
Imports AcurSoft.Numerator.Enums
'Imports AcurSoft.Helpers.Models


' For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
Namespace Controllers

    Partial Public Class ViewControllerForListViewAEx
        Inherits ViewController(Of ListView)
#Region "Numerator Properties"

        Public ReadOnly Property IndexNumeratorHelper As IndexNumeratorHelper
        Public ReadOnly Property IndexNumeratorCreated As Boolean
#Region "Numerator Actions"
        Public ReadOnly Property ActionIndexNumeratorMoveUp As SimpleAction
        Public ReadOnly Property ActionIndexNumeratorMoveDown As SimpleAction

        Public ReadOnly Property ActionIndexNumeratorMoveToTop As SimpleAction
        Public ReadOnly Property ActionIndexNumeratorMoveToLast As SimpleAction
        Public ReadOnly Property ActionIndexNumeratorReorder As SingleChoiceAction
#End Region
#End Region

        Public Sub New()
            InitializeComponent()
            Me.InitNumeratorActions()
            ' Target required Views (via the TargetXXX properties) and create their Actions.
            'TargetViewNesting = Nesting.Nested

        End Sub

#Region "Overrides"

        Protected Overrides Sub OnActivated()
            MyBase.OnActivated()
            _IndexNumeratorHelper = New IndexNumeratorHelper(Me)
            _IndexNumeratorCreated = False
        End Sub
        Protected Overrides Sub OnViewControlsCreated()
            MyBase.OnViewControlsCreated()

            Me.SetupNumeratorActions()
        End Sub

        Protected Overrides Sub OnDeactivated()
            ' Unsubscribe from previously subscribed events and release other references and resources.
            MyBase.OnDeactivated()
            Me.SubscribeIndexNumeratorActionsEvents(False)
        End Sub
#End Region

#Region "Numerator"

        Public Sub InitNumeratorActions()
            _ActionIndexNumeratorMoveUp = New SimpleAction(Me.components)
            With ActionIndexNumeratorMoveUp
                .Id = IndexBaseNumerator.IndexNumeratorName & "_MoveUp"
                .Caption = "Up"
                .ConfirmationMessage = Nothing
                .SelectionDependencyType = SelectionDependencyType.RequireSingleObject
                .ToolTip = "Move Index Up"
                .ImageName = "ArrowToUp"
                '.Category = "Edit"
            End With
            Me.Actions.Add(ActionIndexNumeratorMoveUp)

            _ActionIndexNumeratorMoveDown = New SimpleAction(Me.components)
            With ActionIndexNumeratorMoveDown
                .Id = IndexBaseNumerator.IndexNumeratorName & "_MoveDown"
                .Caption = "Down"
                .ConfirmationMessage = Nothing
                .SelectionDependencyType = SelectionDependencyType.RequireSingleObject
                .ToolTip = "Move Index Down"
                .ImageName = "ArrowToDown"
                '.Category = "Edit"
            End With
            Me.Actions.Add(ActionIndexNumeratorMoveDown)
            _ActionIndexNumeratorMoveToTop = New SimpleAction(Me.components)
            With ActionIndexNumeratorMoveToTop
                .Id = IndexBaseNumerator.IndexNumeratorName & "_ToFirst"
                .Caption = "To First"
                .ConfirmationMessage = Nothing
                .SelectionDependencyType = SelectionDependencyType.Independent
                .ToolTip = "Move Index To First"
                .ImageName = "ArrowToTop"
                '.Category = "Edit"
            End With
            Me.Actions.Add(ActionIndexNumeratorMoveToTop)
            _ActionIndexNumeratorMoveToLast = New SimpleAction(Me.components)
            With ActionIndexNumeratorMoveToLast
                .Id = IndexBaseNumerator.IndexNumeratorName & "_ToLast"
                .Caption = "To Last"
                .ConfirmationMessage = Nothing
                .SelectionDependencyType = SelectionDependencyType.Independent
                .ToolTip = "Move Index To Last"
                .ImageName = "ArrowToLast"
                '.Category = "Edit"
            End With
            Me.Actions.Add(ActionIndexNumeratorMoveToLast)

            _ActionIndexNumeratorReorder = New SingleChoiceAction(Me.components)
            With ActionIndexNumeratorReorder
                .Id = IndexBaseNumerator.IndexNumeratorName & "_Reorder"
                .Caption = "Reorder"
                .ConfirmationMessage = Nothing
                .SelectionDependencyType = SelectionDependencyType.Independent
                .ToolTip = "Reorder"
                .ImageName = "RefreshGreen"
                'System.Enum.GetValues(GetType(ReOrderKind))
                .ItemType = SingleChoiceActionItemType.ItemIsOperation
                .Items.Add(New ChoiceActionItem("Fix Order", ReOrderKind.Fix) With {.ImageName = "RefreshGreen"})
                .Items.Add(New ChoiceActionItem("Reorder By List Sort Order", ReOrderKind.Follow) With {.ImageName = "RefreshOrange"})
                '.Category = "Edit"
            End With
            Me.Actions.Add(ActionIndexNumeratorReorder)
        End Sub

        Public Sub SetupNumeratorActions()
            Me.Frame.GetController(Of DeleteObjectsViewController).DeleteAction.Active.SetItemValue(IndexBaseNumerator.IndexNumeratorName, True)

            If Me.IndexNumeratorHelper.ActionsInfos.IsActive AndAlso Not Me.IndexNumeratorCreated Then
                ActionIndexNumeratorMoveUp.TargetObjectType = Me.IndexNumeratorHelper.ActionsInfos.TargetObjectType
                ActionIndexNumeratorMoveUp.TargetObjectsCriteria = Me.IndexNumeratorHelper.ActionsInfos.CriteriaUp
                ActionIndexNumeratorMoveToTop.TargetObjectType = ActionIndexNumeratorMoveUp.TargetObjectType
                ActionIndexNumeratorMoveToTop.TargetObjectsCriteria = ActionIndexNumeratorMoveUp.TargetObjectsCriteria

                ActionIndexNumeratorMoveDown.TargetObjectType = Me.IndexNumeratorHelper.ActionsInfos.TargetObjectType
                ActionIndexNumeratorMoveDown.TargetObjectsCriteria = Me.IndexNumeratorHelper.ActionsInfos.CriteriaDown
                ActionIndexNumeratorMoveToLast.TargetObjectType = Me.IndexNumeratorHelper.ActionsInfos.TargetObjectType
                ActionIndexNumeratorMoveToLast.TargetObjectsCriteria = ActionIndexNumeratorMoveDown.TargetObjectsCriteria
            End If

            Me.EnableIndexNumeratorActions(Me.IndexNumeratorHelper.IsActive)

        End Sub


        Private Sub SubscribeIndexNumeratorActionsEvents(enable As Boolean)
            If Me.ActionIndexNumeratorMoveUp IsNot Nothing Then
                RemoveHandler Me.ActionIndexNumeratorMoveUp.Execute, AddressOf ActionIndexNumeratorMoveUp_Execute
                If enable Then
                    AddHandler Me.ActionIndexNumeratorMoveUp.Execute, AddressOf ActionIndexNumeratorMoveUp_Execute
                End If
            End If
            If Me.ActionIndexNumeratorMoveDown IsNot Nothing Then
                RemoveHandler Me.ActionIndexNumeratorMoveDown.Execute, AddressOf ActionIndexNumeratorMoveDown_Execute
                If enable Then
                    AddHandler Me.ActionIndexNumeratorMoveDown.Execute, AddressOf ActionIndexNumeratorMoveDown_Execute
                End If
            End If
            If Me.ActionIndexNumeratorMoveToTop IsNot Nothing Then
                RemoveHandler Me.ActionIndexNumeratorMoveToTop.Execute, AddressOf ActionIndexNumeratorMoveToTop_Execute
                If enable Then
                    AddHandler Me.ActionIndexNumeratorMoveToTop.Execute, AddressOf ActionIndexNumeratorMoveToTop_Execute
                End If
            End If
            If Me.ActionIndexNumeratorMoveToLast IsNot Nothing Then
                RemoveHandler Me.ActionIndexNumeratorMoveToLast.Execute, AddressOf ActionIndexNumeratorMoveToLast_Execute
                If enable Then
                    AddHandler Me.ActionIndexNumeratorMoveToLast.Execute, AddressOf ActionIndexNumeratorMoveToLast_Execute
                End If
            End If
            If Me.ActionIndexNumeratorReorder IsNot Nothing Then
                RemoveHandler Me.ActionIndexNumeratorReorder.Execute, AddressOf ActionIndexNumeratorReorder_Execute
                If enable Then
                    AddHandler Me.ActionIndexNumeratorReorder.Execute, AddressOf ActionIndexNumeratorReorder_Execute
                End If
            End If
        End Sub

        Private Sub EnableIndexNumeratorActions(visible As Boolean)
            If Me.IndexNumeratorCreated Then Return
            ActionIndexNumeratorMoveUp.Active.SetItemValue(IndexBaseNumerator.IndexNumeratorName, visible)
            ActionIndexNumeratorMoveDown.Active.SetItemValue(IndexBaseNumerator.IndexNumeratorName, visible)
            ActionIndexNumeratorMoveToTop.Active.SetItemValue(IndexBaseNumerator.IndexNumeratorName, visible)
            ActionIndexNumeratorMoveToLast.Active.SetItemValue(IndexBaseNumerator.IndexNumeratorName, visible)
            ActionIndexNumeratorReorder.Active.SetItemValue(IndexBaseNumerator.IndexNumeratorName, visible)
            Me.SubscribeIndexNumeratorActionsEvents(visible)
            _IndexNumeratorCreated = True
        End Sub

#Region "Order Actions"

        Private Sub ActionIndexNumeratorReorder_Execute(sender As Object, e As SingleChoiceActionExecuteEventArgs)
            Select Case DirectCast(e.SelectedChoiceActionItem.Data, ReOrderKind)
                Case ReOrderKind.Fix
                    Me.IndexNumeratorHelper.ApplyOrder(False)
                Case ReOrderKind.Follow
                    Me.IndexNumeratorHelper.ApplyOrder(Me.View.Editor.GetOrder(ModuleCore.ApplicationPlatform))
            End Select
            'Me.IndexNumeratorHelper.ApplyOrder(False)
        End Sub

        Private Sub ActionIndexNumeratorMoveToLast_Execute(sender As Object, e As SimpleActionExecuteEventArgs)
            Dim count As Integer = Me.View.SelectedObjects.Count
            If count = 0 Then Return
            Dim lst As IEnumerable(Of PersistentBase) = Me.View.SelectedObjects.OfType(Of PersistentBase)
            Dim o As Object = lst.FirstOrDefault
            If o Is Nothing Then Return
            If count = 1 Then
                Me.IndexNumeratorHelper.MoveToLast(o)
            Else
                Me.IndexNumeratorHelper.MoveToLast(lst)
            End If
        End Sub

        Private Sub ActionIndexNumeratorMoveToTop_Execute(sender As Object, e As SimpleActionExecuteEventArgs)
            Dim count As Integer = Me.View.SelectedObjects.Count
            If count = 0 Then Return
            Dim lst As IEnumerable(Of PersistentBase) = Me.View.SelectedObjects.OfType(Of PersistentBase)
            Dim o As Object = lst.FirstOrDefault
            If o Is Nothing Then Return
            If count = 1 Then
                Me.IndexNumeratorHelper.MoveToTop(o)
            Else
                Me.IndexNumeratorHelper.MoveToTop(lst)
            End If
        End Sub

        Private Sub ActionIndexNumeratorMoveDown_Execute(sender As Object, e As SimpleActionExecuteEventArgs)
            Dim o As PersistentBase = Me.View.SelectedObjects.OfType(Of PersistentBase).FirstOrDefault
            If o Is Nothing Then Return
            Me.IndexNumeratorHelper.MoveDown(o)
        End Sub

        Private Sub ActionIndexNumeratorMoveUp_Execute(sender As Object, e As SimpleActionExecuteEventArgs)
            Dim o As PersistentBase = Me.View.SelectedObjects.OfType(Of PersistentBase).FirstOrDefault
            If o Is Nothing Then Return
            Me.IndexNumeratorHelper.MoveUp(o)

        End Sub
#End Region

#End Region
    End Class
End Namespace
