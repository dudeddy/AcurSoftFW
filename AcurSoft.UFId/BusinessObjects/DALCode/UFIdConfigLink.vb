Imports System
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base

Namespace UFId.DAL
    <DefaultClassOptions>
    Partial Public Class UFIdConfigLink
        Public Const PARAM_INDEX As String = "?INDEX"
        Public Const PARAM_DATE As String = "?DATE"
        Public Const PARAM_DATE_EXP As String = "?DATE_EXP"

        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
            Me.DateExpression = "Today()"
            Me.DateTextExpression = String.Format("Year({0})", PARAM_DATE)
            Me.ValueExpression = String.Format("{0} + '_' + {1}", PARAM_DATE_EXP, PARAM_INDEX)
            Me.AppyCondition = "IsNewObject(This)"
        End Sub

        Dim _LastUsedNumValue As Long?
        Public Property LastUsedNumValue() As Long?
            Get
                Return _LastUsedNumValue
            End Get
            Set(ByVal value As Long?)
                SetPropertyValue(Of Long?)("LastUsedNumValue", _LastUsedNumValue, value)
            End Set
        End Property

        Dim _LastUsedDateValue As Date?
        Public Property LastUsedDateValue() As Date?
            Get
                Return _LastUsedDateValue
            End Get
            Set(ByVal value As Date?)
                SetPropertyValue(Of Date?)("LastUsedDateValue", _LastUsedDateValue, value)
            End Set
        End Property

        Public Function GetDraftDateValue(o As BaseObject) As Date?
            Return Me.GetUFIdDateValue(o)
            'Return Me.GetUFIdDateValue(o.Session, o.Oid)
        End Function
        Public Function GetDraftNumValue(o As BaseObject) As Long
            Dim draftDate As Date? = Me.GetDraftDateValue(o)
            Return GetDraftNumValue(draftDate)
        End Function
        Public Function GetDraftNumValue(draftDate As Date?) As Long
            Dim draftDateText As String = Me.GetUFIdDateText(draftDate)
            Return GetDraftNumValue(draftDateText)
        End Function

        Public Function CanApplySequence(o As BaseObject) As Boolean
            Return o.Fit(Me.AppyCondition)
        End Function

        Public Function GetDraftNumValue(draftDateText As String) As Long
            If Me.RestartIfNewDateText Then
                Dim isNewDateText As Boolean = Not Convert.ToBoolean(Me.Evaluate(CriteriaOperator.Parse("UFIdSequences[UFIdDateText = ?].Exists()", draftDateText)))
                If isNewDateText Then
                    Return Me.Config.SequenceStart
                Else
                    Dim cnt As Long = Convert.ToInt64(Me.Evaluate(CriteriaOperator.Parse("UFIdSequences[UFIdDateText = ?].Count()", draftDateText)))
                    If cnt = 0 Then
                        Return Me.Config.SequenceStart
                    Else
                        Return Convert.ToInt64(Me.Evaluate(CriteriaOperator.Parse("UFIdSequences[UFIdDateText = ?].Max(NumValue)", draftDateText))) + Me.Config.SequenceStep
                    End If
                End If
            Else
                If Me.LastUsedNumValue.HasValue Then
                    Return Me.LastUsedNumValue.Value + Me.Config.SequenceStep
                Else
                    Return Me.Config.SequenceStart
                End If
            End If
        End Function
        Public Function GetPatchedDateTextExpression(d As Date) As CriteriaOperator
            Dim v As New UnivesalCriteriaVisitor(Me.DateTextExpression)
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
            Dim v As New UnivesalCriteriaVisitor(Me.ValueExpression)
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

        Public Function GetDraftUFIdText(o As BaseObject) As String
            Dim draftDate As Date? = Me.GetDraftDateValue(o)
            Dim draftDateText As String = Me.GetUFIdDateText(draftDate)
            Dim draftNum As Long? = Me.GetDraftNumValue(draftDateText)

            Return Me.GetDraftUFIdText(o, draftDate, draftDateText, draftNum)
        End Function

        Public Function GetDraftUFIdText(o As BaseObject, draftDate As Date, draftDateText As String, draftNum As Long) As String
            Return Convert.ToString(o.Evaluate(Me.GetPatchedValueExpression(draftNum, draftDateText)))
        End Function

        Public Function IsNewDateText(session As Session, targetId As Guid) As String
            Dim dateText As String = Me.GetUFIdDateText(session, targetId)
            If String.IsNullOrEmpty(dateText) Then Return Nothing
            Return Me.IsNewDateText(session, dateText)
        End Function


        Public Function IsNewDateText(session As Session, dateText As String) As Boolean?
            If String.IsNullOrEmpty(dateText) Then Return Nothing
            Dim link As DAL.UFIdConfigLink = session.GetObjectByKey(Of UFIdConfigLink)(Me.Oid)
            Dim co As CriteriaOperator = CriteriaOperator.Parse("UFIdSequences[UFIdDateText = ?].Exists()", dateText)
            Return DirectCast(link.Evaluate(co), Boolean)
        End Function


        Public Function GetTargetTypeInfo() As ITypeInfo
            If String.IsNullOrEmpty(Me.TargetTypeName) Then Return Nothing
            Return XafTypesInfo.Instance.FindTypeInfo(Me.TargetTypeName)
        End Function

        'Public Function GetTargetObject() As BaseObject
        '    Return Me.GetTargetObject(Me.Session)
        'End Function

        Public Function GetTargetObject(session As Session, targetId As Guid) As BaseObject
            Dim ti As ITypeInfo = Me.GetTargetTypeInfo()
            If ti Is Nothing Then Return Nothing

            Return DirectCast(session.GetObjectByKey(ti.Type, targetId), BaseObject)
        End Function

        Private Function EvaluateUFIdExpression(session As Session, targetId As Guid, expression As String) As Object
            If String.IsNullOrEmpty(expression) Then Return Nothing
            Dim targetObject As BaseObject = Me.GetTargetObject(session, targetId)
            If targetObject Is Nothing Then Return Nothing
            Return EvaluateUFIdExpression(targetObject, expression)
        End Function

        Private Function EvaluateUFIdExpression(targetObject As BaseObject, expression As String) As Object
            If String.IsNullOrEmpty(expression) Then Return Nothing
            If targetObject Is Nothing Then Return Nothing
            Return targetObject.Evaluate(expression)
        End Function


        'Private Function GetUFIdExpression(session As Session, targetId As Guid, expression As String) As String
        '    Return Convert.ToString(Me.EvaluateUFIdExpression(session, targetId, expression))
        'End Function


        Public Function GetUFIdDateValue(session As Session, targetId As Guid) As Date?
            ''DateExpression need processing
            'Throw New NotSupportedException("DateExpression need processing")
            Dim dateObject As Object = Me.EvaluateUFIdExpression(session, targetId, Me.DateExpression)
            If dateObject Is Nothing Then Return Nothing
            Return Convert.ToDateTime(dateObject)
        End Function

        Public Function GetUFIdDateValue(targetObject As BaseObject) As Date?
            ''DateExpression need processing
            'Throw New NotSupportedException("DateExpression need processing")
            Dim dateObject As Object = Me.EvaluateUFIdExpression(targetObject, Me.DateExpression)
            If dateObject Is Nothing Then Return Nothing
            Return Convert.ToDateTime(dateObject)
        End Function

        Public Function GetUFIdDateText(session As Session, targetId As Guid) As String
            If String.IsNullOrEmpty(Me.DateTextExpression) Then Return Nothing

            Dim d As Date? = Me.GetUFIdDateValue(session, targetId)
            If d.HasValue Then
                Return Convert.ToString(Me.GetPatchedDateTextExpression(d.Value).CoreEvaluate())
            End If
            Return Nothing
        End Function

        Public Function GetUFIdDateText(d As Date?) As String
            If String.IsNullOrEmpty(Me.DateTextExpression) Then Return Nothing
            If d.HasValue Then
                Return Convert.ToString(Me.GetPatchedDateTextExpression(d.Value).CoreEvaluate())
            End If
            Return Nothing
        End Function

        Public Function GetUFIdText(session As Session, targetId As Guid) As String
            'ValueExpression need processing
            Throw New NotSupportedException("ValueExpression need processing")

            Return Convert.ToString(Me.EvaluateUFIdExpression(session, targetId, Me.ValueExpression))
        End Function



    End Class

End Namespace
