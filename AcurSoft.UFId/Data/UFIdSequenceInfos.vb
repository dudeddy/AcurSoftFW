Imports DevExpress.Persistent.BaseImpl

Public Class UFIdSequenceInfos
    Public ReadOnly Property DateValue As Date?
    Public ReadOnly Property NumValue As Long?
    Public ReadOnly Property UFIdDateText As String

    Public ReadOnly Property UFIdText As String
    Public ReadOnly Property TargetMember As String

    Public Sub New(o As BaseObject, link As UFId.DAL.UFIdConfigLink)
        Me.TargetMember = link.TargetMember
        Me.DateValue = link.GetDraftDateValue(o)
        Me.UFIdDateText = link.GetUFIdDateText(Me.DateValue)
        Me.NumValue = link.GetDraftNumValue(Me.UFIdDateText)

        Me.UFIdText = link.GetDraftUFIdText(o, Me.DateValue, Me.UFIdDateText, Me.NumValue)

    End Sub

End Class
