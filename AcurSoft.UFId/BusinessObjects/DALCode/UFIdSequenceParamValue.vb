Imports System
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports System.ComponentModel
Namespace UFId.DAL

    Partial Public Class UFIdSequenceParamValue
        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
        End Sub


        Public Function SetParamValue() As String
            If String.IsNullOrEmpty(Me.LinkParam?.Expression) Then
                Me.ParamValue = String.Empty
            Else
                Dim objectParamValue = Me.LinkParam.Expression.CoreEvaluate()
                Me.ParamValue = Me.LinkParam.Type.ObjectToString(objectParamValue)
            End If
            Return Me.ParamValue
        End Function

    End Class

End Namespace
