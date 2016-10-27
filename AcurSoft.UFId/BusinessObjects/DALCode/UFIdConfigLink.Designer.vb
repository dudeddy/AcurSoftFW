﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------
Imports System
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports System.ComponentModel
Namespace UFId.DAL

    Partial Public Class UFIdConfigLink
        Inherits DevExpress.Persistent.BaseImpl.BaseObject
        Dim _Code As String
        Public Property Code() As String
            Get
                Return _Code
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("Code", _Code, value)
            End Set
        End Property
        Dim _Config As UFIdConfig
        <Association("UFIdConfigLinkReferencesUFIdConfig")>
        Public Property Config() As UFIdConfig
            Get
                Return _Config
            End Get
            Set(ByVal value As UFIdConfig)
                SetPropertyValue(Of UFIdConfig)("Config", _Config, value)
            End Set
        End Property
        Dim _TargetTypeName As String
        Public Property TargetTypeName() As String
            Get
                Return _TargetTypeName
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("TargetTypeName", _TargetTypeName, value)
            End Set
        End Property
        Dim _TargetMember As String
        Public Property TargetMember() As String
            Get
                Return _TargetMember
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("TargetMember", _TargetMember, value)
            End Set
        End Property
        Dim _DateExpression As String
        <Size(SizeAttribute.Unlimited)>
        Public Property DateExpression() As String
            Get
                Return _DateExpression
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("DateExpression", _DateExpression, value)
            End Set
        End Property
        Dim _DateTextExpression As String
        <Size(SizeAttribute.Unlimited)>
        Public Property DateTextExpression() As String
            Get
                Return _DateTextExpression
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("DateTextExpression", _DateTextExpression, value)
            End Set
        End Property
        Dim _ValueExpression As String
        <Size(SizeAttribute.Unlimited)>
        Public Property ValueExpression() As String
            Get
                Return _ValueExpression
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("ValueExpression", _ValueExpression, value)
            End Set
        End Property
        Dim _RestartIfNewDateText As Boolean
        Public Property RestartIfNewDateText() As Boolean
            Get
                Return _RestartIfNewDateText
            End Get
            Set(ByVal value As Boolean)
                SetPropertyValue(Of Boolean)("RestartIfNewDateText", _RestartIfNewDateText, value)
            End Set
        End Property
        Dim _StoreUFIdText As Boolean
        Public Property StoreUFIdText() As Boolean
            Get
                Return _StoreUFIdText
            End Get
            Set(ByVal value As Boolean)
                SetPropertyValue(Of Boolean)("StoreUFIdText", _StoreUFIdText, value)
            End Set
        End Property
        Dim _AppyCondition As String
        <Size(SizeAttribute.Unlimited)>
        Public Property AppyCondition() As String
            Get
                Return _AppyCondition
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("AppyCondition", _AppyCondition, value)
            End Set
        End Property
        <Association("UFIdSequenceReferencesUFIdConfigLink"), Aggregated()>
        Public ReadOnly Property UFIdSequences() As XPCollection(Of UFIdSequence)
            Get
                Return GetCollection(Of UFIdSequence)("UFIdSequences")
            End Get
        End Property
    End Class

End Namespace