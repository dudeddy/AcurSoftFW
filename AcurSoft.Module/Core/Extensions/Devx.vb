Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base

Module Devx
    Public Const CurrentViewKey As String = "View"
    <Extension()>
    Public Function GetDictionaryTag(ByVal view As View) As Dictionary(Of String, Object)
        Dim dic As Dictionary(Of String, Object) = Nothing
        If view.Tag Is Nothing OrElse view.Tag.GetType IsNot GetType(Dictionary(Of String, Object)) Then
            dic = New Dictionary(Of String, Object)
            view.Tag = dic
        Else
            dic = DirectCast(view.Tag, Dictionary(Of String, Object))
        End If
        If Not dic.ContainsKey(Devx.CurrentViewKey) Then
            dic(Devx.CurrentViewKey) = view
        End If
        view.Tag = dic
        Return DirectCast(view.Tag, Dictionary(Of String, Object))
    End Function


    <Extension()>
    Public Function GetObjectSpace(ByVal viewController As ViewController) As IObjectSpace
        Return DirectCast(GetType(ViewController).GetProperty("ObjectSpace", BindingFlags.Instance Or BindingFlags.NonPublic).GetValue(viewController), IObjectSpace)
    End Function
End Module
