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
Namespace DAL

    Partial Public Class ObjectTemplate
        Inherits DevExpress.Persistent.BaseImpl.BaseObject
        Dim _TargetTypeName As String
        <DevExpress.ExpressApp.Model.ModelDefault("AllowEdit", "False")>
        Public Property TargetTypeName() As String
            Get
                Return _TargetTypeName
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("TargetTypeName", _TargetTypeName, value)
            End Set
        End Property
        Dim _Code As String
        Public Property Code() As String
            Get
                Return _Code
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("Code", _Code, value)
            End Set
        End Property
        Dim _Caption As String
        Public Property Caption() As String
            Get
                Return _Caption
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("Caption", _Caption, value)
            End Set
        End Property
        <Association("ObjectTemplateMemberReferencesObjectTemplate"), Aggregated()>
        Public ReadOnly Property ObjectTemplateMembers() As XPCollection(Of ObjectTemplateMember)
            Get
                Return GetCollection(Of ObjectTemplateMember)("ObjectTemplateMembers")
            End Get
        End Property
    End Class

End Namespace