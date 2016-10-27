Imports System
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.BaseImpl

Namespace UFId.DAL

    Partial Public Class UFIdSequence
        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
        End Sub

        <PersistentAlias("True")>
        Public ReadOnly Property PersistentAlias2() As String
            Get
                Return CType(EvaluateAlias("PersistentAlias2"), String)
            End Get
        End Property

        Public Function GetTargetTypeInfo() As ITypeInfo
            If String.IsNullOrEmpty(Me.Link.TargetTypeName) Then Return Nothing
            Return XafTypesInfo.Instance.FindTypeInfo(Me.Link.TargetTypeName)
        End Function

        Public Function GetTargetObject() As BaseObject
            Return Me.GetTargetObject(Me.Session)
        End Function

        Public Function GetTargetObject(session As Session) As BaseObject
            If String.IsNullOrEmpty(Me.Link?.TargetTypeName) Then Return Nothing
            Dim ti As ITypeInfo = Me.GetTargetTypeInfo()
            If ti Is Nothing Then Return Nothing

            Return DirectCast(session.GetObjectByKey(ti.Type, Me.TargetId), BaseObject)
        End Function

        Private Function GetUFIdExpression(session As Session, expression As String) As String
            If String.IsNullOrEmpty(expression) Then Return Nothing
            Dim targetObject As BaseObject = Me.GetTargetObject(session)
            If targetObject Is Nothing Then Return Nothing
            Return Convert.ToString(targetObject.Evaluate(expression))
        End Function

        Public Function GetUFIdDateText(session As Session) As String
            Return Me.GetUFIdExpression(session, Me.Link?.DateExpression)
        End Function

        Public Function GetUFIdText(session As Session) As String
            Return Me.GetUFIdExpression(session, Me.Link?.ValueExpression)
        End Function



        Public ReadOnly Property PersistentAlias3() As String
            Get
                Dim x = Me.Link.TargetTypeName
                Dim id = Me.TargetId
                Dim ti As ITypeInfo = XafTypesInfo.Instance.FindTypeInfo(Me.Link.TargetTypeName)
                Return CType(EvaluateAlias("PersistentAlias2"), String)
            End Get
        End Property
    End Class

End Namespace
