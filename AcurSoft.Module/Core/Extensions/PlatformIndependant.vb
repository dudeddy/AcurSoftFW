Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports AcurSoft.ModuleCore
Imports DevExpress.Data
Imports DevExpress.ExpressApp.Editors
Imports AcurSoft.Data
Imports AcurSoft.Enums

Module CrossPlatform

    <Extension()>
    Public Function GetOrder(listEditor As ListEditor, platformKind As PlatformKind) As List(Of ColumnSortOrderInfo)
        Dim data As New List(Of ColumnSortOrderInfo)
        If listEditor.Control Is Nothing Then Return data
        Dim editorType As Type = listEditor.Control.GetType
        If platformKind = PlatformKind.Web Then
            Dim grid As Object = listEditor.Control '?.Item(fieldName)
            Dim piSortedColumns As PropertyInfo = editorType.GetProperty("SortedColumns", BindingFlags.NonPublic Or BindingFlags.Instance)
            If piSortedColumns IsNot Nothing Then
                Dim sortedColumns As IEnumerable(Of Object) = DirectCast(piSortedColumns.GetValue(listEditor.Control), IEnumerable).OfType(Of Object)
                If sortedColumns IsNot Nothing AndAlso sortedColumns.Count > 0 Then
                    For Each sc In sortedColumns
                        data.Add(New ColumnSortOrderInfo(Convert.ToInt32(sc.SortIndex), sc.FieldName.ToString(), CType(sc.SortOrder, ColumnSortOrder)))
                    Next
                End If
            End If
        ElseIf platformKind = PlatformKind.Win Then
            Dim sortInfo As Object = listEditor.Control?.FocusedView?.SortInfo
            If sortInfo IsNot Nothing Then
                If TypeOf sortInfo IsNot CollectionBase Then Return data

                Dim collection As CollectionBase = DirectCast(sortInfo, CollectionBase)
                Dim cnt As Integer = collection.Count - 1
                For i As Integer = 0 To cnt
                    Dim elm As Object = collection(i)
                    Dim sortOrder As ColumnSortOrder = DirectCast(elm.SortOrder, ColumnSortOrder)
                    Dim fieldName As String = CType(elm.Column?.FieldName, String)
                    Dim caption As String = CType(elm.Column?.Caption, String)
                    data.Add(New ColumnSortOrderInfo(i, fieldName, sortOrder) With {.Caption = caption})
                Next
            End If
        End If
        Return data
    End Function


    <Extension()>
    Public Sub SetOrderBy(listEditor As ListEditor, platformKind As PlatformKind, fieldName As String, sortOrder As DevExpress.Data.ColumnSortOrder)
        If listEditor.Control Is Nothing Then Return
        Dim editorType As Type = listEditor.Control.GetType
        If platformKind = PlatformKind.Web Then
            Dim column As Object = listEditor.Control.Columns?.Item(fieldName)
            If column IsNot Nothing Then
                Dim piSortOrder As PropertyInfo = column.GetType().GetProperty("SortOrder")
                If piSortOrder IsNot Nothing Then
                    piSortOrder.SetValue(column, sortOrder)
                End If
            End If
        ElseIf platformKind = PlatformKind.Win Then
            Dim column As Object = listEditor.Control?.FocusedView?.Columns?.Item(fieldName)
            If column IsNot Nothing Then
                Dim piSortOrder As PropertyInfo = column.GetType().GetProperty("SortOrder")
                If piSortOrder IsNot Nothing Then
                    piSortOrder.SetValue(column, sortOrder)
                End If
            End If
        End If
    End Sub


    <Extension()>
    Public Sub SetFormat(listEditor As ListEditor, platformKind As PlatformKind, fieldName As String, formatMask As String, Optional editMaskType As EditMaskType = EditMaskType.Default)
        If listEditor.Control Is Nothing Then Return
        Dim editorType As Type = listEditor.Control.GetType
        If platformKind = PlatformKind.Web Then
            Dim propertiesEdit As Object = listEditor.Control.Columns?.Item(fieldName)?.PropertiesEdit '?.DisplayFormatString
            If propertiesEdit IsNot Nothing Then
                Dim piDisplayFormatString As PropertyInfo = propertiesEdit.GetType().GetProperty("DisplayFormatString")
                piDisplayFormatString.SetValue(propertiesEdit, formatMask)
            End If
        ElseIf platformKind = PlatformKind.Win Then
            Dim displayFormat As Object = listEditor.Control?.FocusedView?.Columns?.Item(fieldName)?.DisplayFormat
            If displayFormat IsNot Nothing Then
                Dim displayFormatType As Type = displayFormat.GetType()
                Dim piFormatString As PropertyInfo = displayFormatType.GetProperty("FormatString")
                If piFormatString IsNot Nothing Then
                    piFormatString.SetValue(displayFormat, formatMask)
                End If
                Dim editMaskTypeInt As Integer = -1
                Select Case editMaskType
                    Case EditMaskType.Default
                    Case EditMaskType.DateTime
                        editMaskTypeInt = 2
                        'Case EditMaskType.RegEx
                        '    editMaskTypeInt = 4
                        'Case EditMaskType.Simple
                        '    editMaskTypeInt = 6
                End Select
                If editMaskTypeInt <> -1 Then
                    Dim piFormatType As PropertyInfo = displayFormatType.GetProperty("FormatType")
                    If piFormatType IsNot Nothing Then
                        piFormatType.SetValue(displayFormat, editMaskTypeInt)
                    End If
                End If
            End If
        End If
    End Sub



    <Extension()>
    Public Sub SetFormat(propertyEditor As PropertyEditor, platformKind As PlatformKind, Optional formatMask As String = Nothing, Optional editMaskType As EditMaskType = EditMaskType.Default)
        If propertyEditor.Control Is Nothing Then
            If platformKind = PlatformKind.Web Then
                Dim piDisplayFormat As PropertyInfo = propertyEditor.GetType().GetProperty("DisplayFormat")
                If piDisplayFormat IsNot Nothing Then
                    piDisplayFormat.SetValue(propertyEditor, formatMask)
                End If
            End If
            Return
        End If
        Dim editorType As Type = propertyEditor.Control.GetType
        If formatMask Is Nothing Then
            formatMask = propertyEditor.EditMask
        End If
        If editMaskType = EditMaskType.Default Then
            editMaskType = propertyEditor.EditMaskType
        End If
        propertyEditor.EditMask = formatMask
        propertyEditor.EditMaskType = editMaskType

        If platformKind = PlatformKind.Win Then
            Dim mask As Object = propertyEditor.Control.Properties?.Mask
            If mask IsNot Nothing Then
                Dim maskType As Type = mask.GetType()

                Dim piEditMask As PropertyInfo = maskType.GetProperty("EditMask")
                If piEditMask IsNot Nothing Then
                    piEditMask.SetValue(mask, formatMask)
                End If
                Dim editMaskTypeInt As Integer = -1
                Select Case editMaskType
                    Case EditMaskType.Default
                    Case EditMaskType.DateTime
                        editMaskTypeInt = 1
                    Case EditMaskType.RegEx
                        editMaskTypeInt = 4
                    Case EditMaskType.Simple
                        editMaskTypeInt = 6
                End Select
                If editMaskTypeInt <> -1 Then
                    Dim piMaskType As PropertyInfo = maskType.GetProperty("MaskType")
                    If piMaskType IsNot Nothing Then
                        piMaskType.SetValue(mask, editMaskTypeInt)
                    End If
                End If
                Dim piUseMaskAsDisplayFormat As PropertyInfo = maskType.GetProperty("UseMaskAsDisplayFormat")
                If piUseMaskAsDisplayFormat IsNot Nothing Then
                    piUseMaskAsDisplayFormat.SetValue(mask, True)
                End If
            End If
        ElseIf platformKind = PlatformKind.Web
            Dim propertyEditorObject As Object = propertyEditor

            'Dim xx As Object = propertyEditorObject.Editor?.DisplayFormatString
            Dim editor As Object = propertyEditorObject.Editor
            If editor IsNot Nothing Then
                Dim piDisplayFormatString As PropertyInfo = editor.GetType().GetProperty("DisplayFormatString")
                If piDisplayFormatString IsNot Nothing Then
                    piDisplayFormatString.SetValue(editor, formatMask)
                End If

            End If

        End If
    End Sub

End Module
