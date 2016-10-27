Imports AcurSoft.ModuleCore
Imports DevExpress.ExpressApp.Editors

Imports System.Reflection
Imports AcurSoft.Enums

Public Class ListEditorHelper
    Public Event OnSortEnd(sender As Object, e As EventArgs)
    Public ReadOnly Property ListEditor As ListEditor
    Public ReadOnly Property PlatformKind As PlatformKind
    Public Sub New(listEditor As ListEditor, platformKind As PlatformKind)
        Me.ListEditor = listEditor
        Me.PlatformKind = platformKind
        If Me.PlatformKind = PlatformKind.Win Then
            Dim gv = listEditor.Control.FocusedView

            Dim endSortingEventInfo As EventInfo = gv.GetType().GetEvent("EndSorting")
            Dim handler As Action(Of Object, Object) = AddressOf Grid_EndSort
            endSortingEventInfo.AddEventHandler(gv, ConvertDelegate(handler, endSortingEventInfo.EventHandlerType))
        ElseIf Me.PlatformKind = PlatformKind.Web
            Dim gv = listEditor.Control

            Dim eventInfo As EventInfo = gv.GetType().GetEvent("CustomColumnSort")
            Dim handler As Action(Of Object, Object) = AddressOf Grid_EndSort
            Dim convertedHandler As System.Delegate = ConvertDelegate(handler, eventInfo.EventHandlerType)
            eventInfo.AddEventHandler(gv, convertedHandler)

        End If

    End Sub

    Public Function ConvertDelegate(ByVal originalDelegate As System.Delegate, ByVal targetDelegateType As Type) As System.Delegate
        Return System.Delegate.CreateDelegate(targetDelegateType, originalDelegate.Target, originalDelegate.Method)
    End Function

    Public Sub Grid_EndSort(sender As Object, e As EventArgs)
        RaiseEvent OnSortEnd(sender, e)
    End Sub

End Class
