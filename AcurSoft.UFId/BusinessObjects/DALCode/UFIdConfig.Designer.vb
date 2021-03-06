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

    Partial Public Class UFIdConfig
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
        Dim _Name As String
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                SetPropertyValue(Of String)("Name", _Name, value)
            End Set
        End Property
        Dim _ReUseDeletedStrategy As AcurSoft.UFId.Enums.ReUseStrategies
        Public Property ReUseDeletedStrategy() As AcurSoft.UFId.Enums.ReUseStrategies
            Get
                Return _ReUseDeletedStrategy
            End Get
            Set(ByVal value As AcurSoft.UFId.Enums.ReUseStrategies)
                SetPropertyValue(Of AcurSoft.UFId.Enums.ReUseStrategies)("ReUseDeletedStrategy", _ReUseDeletedStrategy, value)
            End Set
        End Property
        Dim _AllowReservation As Boolean
        Public Property AllowReservation() As Boolean
            Get
                Return _AllowReservation
            End Get
            Set(ByVal value As Boolean)
                SetPropertyValue(Of Boolean)("AllowReservation", _AllowReservation, value)
            End Set
        End Property
        Dim _AffectReservedStrategy As AcurSoft.UFId.Enums.ReUseStrategies
        Public Property AffectReservedStrategy() As AcurSoft.UFId.Enums.ReUseStrategies
            Get
                Return _AffectReservedStrategy
            End Get
            Set(ByVal value As AcurSoft.UFId.Enums.ReUseStrategies)
                SetPropertyValue(Of AcurSoft.UFId.Enums.ReUseStrategies)("AffectReservedStrategy", _AffectReservedStrategy, value)
            End Set
        End Property
        Dim _DeletedAndReservedStrategy As AcurSoft.UFId.Enums.DeletedAndReservedStrategies
        Public Property DeletedAndReservedStrategy() As AcurSoft.UFId.Enums.DeletedAndReservedStrategies
            Get
                Return _DeletedAndReservedStrategy
            End Get
            Set(ByVal value As AcurSoft.UFId.Enums.DeletedAndReservedStrategies)
                SetPropertyValue(Of AcurSoft.UFId.Enums.DeletedAndReservedStrategies)("DeletedAndReservedStrategy", _DeletedAndReservedStrategy, value)
            End Set
        End Property
        Dim _SequenceStart As Long
        Public Property SequenceStart() As Long
            Get
                Return _SequenceStart
            End Get
            Set(ByVal value As Long)
                SetPropertyValue(Of Long)("SequenceStart", _SequenceStart, value)
            End Set
        End Property
        Dim _SequenceStep As Long
        Public Property SequenceStep() As Long
            Get
                Return _SequenceStep
            End Get
            Set(ByVal value As Long)
                SetPropertyValue(Of Long)("SequenceStep", _SequenceStep, value)
            End Set
        End Property
        Dim _SequenceEnd As Long
        Public Property SequenceEnd() As Long
            Get
                Return _SequenceEnd
            End Get
            Set(ByVal value As Long)
                SetPropertyValue(Of Long)("SequenceEnd", _SequenceEnd, value)
            End Set
        End Property
        <Association("UFIdConfigLinkReferencesUFIdConfig"), Aggregated()>
        Public ReadOnly Property UFIdConfigLinks() As XPCollection(Of UFIdConfigLink)
            Get
                Return GetCollection(Of UFIdConfigLink)("UFIdConfigLinks")
            End Get
        End Property
    End Class

End Namespace
