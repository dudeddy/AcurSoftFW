
Imports AcurSoft.UFId.Enums
Namespace UFId.Utils
    Public Class UFIdNumValueInfos


        Public ReadOnly Property IsNewDateText As Boolean
        Public ReadOnly Property HasDeleted As Boolean
        Public ReadOnly Property HasReserved As Boolean
        Public Property Value As Long
        Public ReadOnly Property Link As UFId.DAL.UFIdConfigLink
        Public ReadOnly Property DeletedSequences As IEnumerable(Of UFId.DAL.UFIdSequence)
        'Public ReadOnly Property DeletedSequenceToUse As UFId.DAL.UFIdSequence

        Public ReadOnly Property ReservedSequences As IEnumerable(Of UFId.DAL.UFIdSequence)

        'Public ReadOnly Property ReservedSequenceToUse As UFId.DAL.UFIdSequence
        Public ReadOnly Property SequenceToUse As UFId.DAL.UFIdSequence
        Public ReadOnly Property StatusToUse As SequenceStatuses
        'Public ReadOnly Property SequenceMayUse As UFId.DAL.UFIdSequence

        Public ReadOnly Property NeedToAsk As Boolean
        'Public ReadOnly Property IsUniqueValue As Boolean
        Public ReadOnly Property ChoiceSequences As List(Of UFId.DAL.UFIdSequence)

        'Public ReadOnly Property CanAutoGenerate As Boolean


        Public Sub New(link As UFId.DAL.UFIdConfigLink)
            Me.Link = link
            Me.ChoiceSequences = New List(Of UFId.DAL.UFIdSequence)
            'Me.IsUniqueValue = Me.Link.Config.ReUseDeletedStrategy <> ReUseStrategies.Ask AndAlso Me.Link.Config.AffectReservedStrategy <> ReUseStrategies.Ask
        End Sub
        Public Sub New(link As UFId.DAL.UFIdConfigLink, draftDateText As String)
            Me.New(link)
            Me.Init(draftDateText)
        End Sub

        Public Sub Init(draftDateText As String)
            _StatusToUse = SequenceStatuses.Draft
            If Me.Link.RestartIfNewDateText Then
                'Return Me.LastUsedNumValue.Value + Me.Config.SequenceStep
                _IsNewDateText = Not Me.Link.UFIdSequences.Any(Function(q) q.UFIdDateText = draftDateText)

            Else
                _IsNewDateText = False
            End If

            If Me.IsNewDateText Then
                _Value = Me.Link.Config.SequenceStart
            Else
                If Me.Link.Config.ReUseDeletedStrategy <> ReUseStrategies.None Then
                    _DeletedSequences = Me.Link.DeletedSequences.Where(Function(q) q.UFIdDateText = draftDateText) '.OrderBy(Function(q) q.NumValue)
                    _HasDeleted = _DeletedSequences.Count > 0
                End If
                If Me.Link.Config.AllowReservation AndAlso Me.Link.Config.AffectReservedStrategy <> ReUseStrategies.None Then
                    _ReservedSequences = Me.Link.ReservedSequences.Where(Function(q) q.UFIdDateText = draftDateText) '.OrderBy(Function(q) q.NumValue)
                    _HasReserved = _ReservedSequences.Count > 0
                End If

                If Me.HasDeleted Then
                    Select Case Me.Link.Config.ReUseDeletedStrategy
                        Case ReUseStrategies.ReUseFirst
                            Me.ChoiceSequences.Add(Me.DeletedSequences.FirstOrDefault)
                        Case ReUseStrategies.ReUseLast
                            Me.ChoiceSequences.Add(Me.DeletedSequences.LastOrDefault)
                        Case ReUseStrategies.Ask
                            Me.ChoiceSequences.AddRange(Me.DeletedSequences)
                            _NeedToAsk = True
                    End Select
                End If
                If Me.HasReserved Then
                    Select Case Me.Link.Config.AffectReservedStrategy
                        Case ReUseStrategies.ReUseFirst
                            Me.ChoiceSequences.Add(Me.ReservedSequences.FirstOrDefault)
                        Case ReUseStrategies.ReUseLast
                            Me.ChoiceSequences.Add(Me.ReservedSequences.LastOrDefault)
                        Case ReUseStrategies.Ask
                            Me.ChoiceSequences.AddRange(Me.ReservedSequences)
                            _NeedToAsk = True
                    End Select
                End If

                If Me.HasDeleted AndAlso Me.HasReserved Then
                    Select Case Me.Link.Config.DeletedAndReservedStrategy
                        Case DeletedAndReservedStrategies.UseDeleted
                            _SequenceToUse = Me.ChoiceSequences.FirstOrDefault(Function(q) q.Status = SequenceStatuses.Deleted)
                            _StatusToUse = SequenceStatuses.Deleted
                        Case DeletedAndReservedStrategies.UseReserved
                            _SequenceToUse = Me.ChoiceSequences.FirstOrDefault(Function(q) q.Status = SequenceStatuses.Reserved)
                            _StatusToUse = SequenceStatuses.Reserved
                        Case DeletedAndReservedStrategies.Ask
                            _NeedToAsk = True
                            _SequenceToUse = Me.ChoiceSequences.FirstOrDefault(Function(q) q.Status = SequenceStatuses.Deleted)
                            _StatusToUse = SequenceStatuses.Deleted
                    End Select
                ElseIf Me.HasDeleted
                    _SequenceToUse = Me.ChoiceSequences.FirstOrDefault(Function(q) q.Status = SequenceStatuses.Deleted)
                    _StatusToUse = SequenceStatuses.Deleted
                ElseIf Me.HasReserved
                    _SequenceToUse = Me.ChoiceSequences.FirstOrDefault(Function(q) q.Status = SequenceStatuses.Reserved)
                    _StatusToUse = SequenceStatuses.Reserved
                Else
                    _StatusToUse = SequenceStatuses.Draft
                    'Calculated 
                End If
                If Me.StatusToUse = SequenceStatuses.Draft Then
                    Dim count As Long = Me.Link.UFIdSequences.LongCount(Function(q) q.UFIdDateText = draftDateText)
                    If count = 0 Then
                        _Value = Me.Link.Config.SequenceStart
                    Else
                        _Value = Me.Link.UFIdSequences.Where(Function(q) q.UFIdDateText = draftDateText).Max(Function(q) q.NumValue) + Me.Link.Config.SequenceStep
                    End If
                Else
                    _Value = Me.SequenceToUse.NumValue
                End If
            End If
        End Sub


    End Class
End Namespace
