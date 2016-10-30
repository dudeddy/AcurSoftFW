Imports AcurSoft.Data.Filtering
Imports AcurSoft.UFId.DAL
Imports DevExpress.Data.Filtering
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Xpo

Namespace UFId.Utils
    Public Class UFIdSequenceInfos
        Public Const PARAM_INDEX As String = "?INDEX"
        Public Const PARAM_DATE As String = "?DATE"
        Public Const PARAM_DATE_EXP As String = "?DATE_EXP"

        Public ReadOnly Property DateValue As Date?
        'Public ReadOnly Property NumValue As Long?
        Public ReadOnly Property UFIdDateText As String

        'Public ReadOnly Property UFIdText As String
        Public ReadOnly Property Link As UFId.DAL.UFIdConfigLink
        Public ReadOnly Property Target As BaseObject

        Public ReadOnly Property TargetMember As String
        Public ReadOnly Property ValueInfos As UFIdNumValueInfos

        Public ReadOnly Property NeedToAsk As Boolean


        Public Sub New(o As BaseObject, link As UFId.DAL.UFIdConfigLink)
            Me.Link = link
            Me.Target = o
            Me.TargetMember = link.TargetMember
            Me.DateValue = Me.GetUFIdDateValue(o)
            Me.UFIdDateText = Me.GetUFIdDateText(Me.DateValue)
            Me.ValueInfos = New UFIdNumValueInfos(link, Me.UFIdDateText)
            Me.NeedToAsk = ValueInfos.NeedToAsk

        End Sub

#Region "Helpers"

        Private Function EvaluateUFIdExpression(targetObject As BaseObject, expression As String) As Object
            If String.IsNullOrEmpty(expression) Then Return Nothing
            If targetObject Is Nothing Then Return Nothing
            Return targetObject.Evaluate(expression)
        End Function


        Public Function GetUFIdDateValue(targetObject As BaseObject) As Date?
            ''DateExpression need processing
            'Throw New NotSupportedException("DateExpression need processing")
            Dim dateObject As Object = Me.EvaluateUFIdExpression(targetObject, Me.Link.DateExpression)
            If dateObject Is Nothing Then Return Nothing
            Return Convert.ToDateTime(dateObject)
        End Function

        Public Function GetUFIdDateText(d As Date?) As String
            If String.IsNullOrEmpty(Me.Link.DateTextExpression) Then Return Nothing
            If d.HasValue Then
                Return Convert.ToString(Me.GetPatchedDateTextExpression(d.Value).CoreEvaluate())
            End If
            Return Nothing
        End Function

        Public Function GetPatchedDateTextExpression(d As Date) As CriteriaOperator
            Dim v As New UnivesalCriteriaVisitor(Me.Link.DateTextExpression)
            AddHandler v.OnVisiting,
                Sub(e)
                    If TypeOf e.OriginalCriteria Is OperandParameter Then
                        Dim op As OperandParameter = DirectCast(e.OriginalCriteria, OperandParameter)
                        If "?" & op.ParameterName.ToUpper = PARAM_DATE Then
                            e.Handled = True
                            op.Value = d
                            e.NewCriteria = op
                        End If
                    End If
                End Sub
            Return v.Process()
        End Function

        Public Function GetPatchedValueExpression(num As Long, dateText As String) As CriteriaOperator
            Dim v As New UnivesalCriteriaVisitor(Me.Link.ValueExpression)
            AddHandler v.OnVisiting,
                Sub(e)
                    If TypeOf e.OriginalCriteria Is OperandParameter Then
                        Dim op As OperandParameter = DirectCast(e.OriginalCriteria, OperandParameter)
                        If "?" & op.ParameterName.ToUpper = PARAM_INDEX Then
                            e.Handled = True
                            op.Value = num
                            e.NewCriteria = op
                        ElseIf "?" & op.ParameterName.ToUpper = PARAM_DATE_EXP Then
                            e.Handled = True
                            op.Value = dateText
                            e.NewCriteria = op
                        ElseIf "?" & op.ParameterName.ToUpper = PARAM_DATE Then
                            e.Handled = True
                            op.Value = dateText
                            e.NewCriteria = op
                        End If
                    End If
                End Sub
            Return v.Process()
        End Function

#End Region

        Public Function GetUFIdText() As String
            Return Convert.ToString(Me.Target.Evaluate(Me.GetPatchedValueExpression(Me.ValueInfos.Value, Me.UFIdDateText)))
        End Function

        Public Function CreateSequence(session As Session) As UFIdSequence
            Dim seq As New UFIdSequence(session) With {
            .TargetId = Me.Target.Oid,
            .Link = Link,
            .Status = UFId.Enums.SequenceStatuses.Assigned,
            .DateValue = Me.DateValue.Value,
            .UFIdDateText = Me.UFIdDateText,
            .NumValue = Me.ValueInfos.Value,
            .UFIdText = Me.GetUFIdText()
        }
            Return seq

            'If Me.ValueInfos.NeedToAsk Then
            'Else
            'End If


        End Function

    End Class
End Namespace
