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

    Partial Public Class ObjectTemplateChildMemberValue
        Inherits XPObject
        Dim _Id As Integer
        Public Property Id() As Integer
            Get
                Return _Id
            End Get
            Set(ByVal value As Integer)
                SetPropertyValue(Of Integer)("Id", _Id, value)
            End Set
        End Property
        Dim _Member As ObjectTemplateMember
        <Association("ObjectTemplateChildMemberValueReferencesObjectTemplateMember")>
        Public Property Member() As ObjectTemplateMember
            Get
                Return _Member
            End Get
            Set(ByVal value As ObjectTemplateMember)
                SetPropertyValue(Of ObjectTemplateMember)("Member", _Member, value)
            End Set
        End Property
        Dim _ChildMember As ObjectTemplateChildMember
        <Association("ObjectTemplateChildMemberValueReferencesObjectTemplateChildMember")>
        Public Property ChildMember() As ObjectTemplateChildMember
            Get
                Return _ChildMember
            End Get
            Set(ByVal value As ObjectTemplateChildMember)
                SetPropertyValue(Of ObjectTemplateChildMember)("ChildMember", _ChildMember, value)
            End Set
        End Property
        Dim _MemberValue As String
        <Size(SizeAttribute.Unlimited)>
        Public Property MemberValue() As String
            Get
                Return _MemberValue
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("MemberValue", _MemberValue, value)
            End Set
        End Property
    End Class

End Namespace