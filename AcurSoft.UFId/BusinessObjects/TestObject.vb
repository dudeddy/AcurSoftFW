Imports System
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.Persistent.Base
Imports AcurSoft.Enums
Imports AcurSoft.UFId.Utils

Namespace UFId.DAL

    <DefaultClassOptions>
    Partial Public Class TestObject
        'Private _IsHandlingTransaction As Boolean
        'Private Shared _SequenceGenerator As SequenceGenerator
        'Private Shared _SyncRoot As New Object()
        'Public ReadOnly Property UseSequenceGenerator As Boolean?
        Private _UFIdHandler As UFIdHelper
        <Browsable(False)>
        Public ReadOnly Property UFIdHandler As UFIdHelper
            Get
                If _UFIdHandler Is Nothing Then
                    _UFIdHandler = New UFIdHelper(Me)
                End If
                Return _UFIdHandler
            End Get
        End Property



        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
            'Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.JustCreated))
            ''Me.SetDraftSequenceValues()
            'Me.UFIdHandler.SetDraftSequenceValues()
        End Sub

        '#Region "Sequence"

        '        Protected Overrides Sub OnSaving()
        '            Dim editStautusOriginal As ObjectEditStatuses = Me.EditStatus.Status
        '            MyBase.OnSaving()
        '            If editStautusOriginal = ObjectEditStatuses.JustCreated Then
        '                Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.NewSaved))
        '            End If
        '            Try
        '                MyBase.OnSaving()
        '                If editStautusOriginal = ObjectEditStatuses.JustCreated Then
        '                    Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.NewSaved))
        '                End If
        '                'If Not (TypeOf Session Is NestedUnitOfWork) AndAlso (Session.DataLayer IsNot Nothing) AndAlso (TypeOf Session.ObjectLayer Is SimpleObjectLayer) AndAlso Session.IsNewObject(Me) Then
        '                Me.UFIdHandler.TrySave()
        '                'If Not (TypeOf Session Is NestedUnitOfWork) AndAlso (Session.DataLayer IsNot Nothing) AndAlso (TypeOf Session.ObjectLayer Is SimpleObjectLayer) Then
        '                '    'OR
        '                '    '&& !(Session.ObjectLayer is DevExpress.ExpressApp.Security.ClientServer.SecuredSessionObjectLayer)
        '                '    'Dim co As CriteriaOperator = CriteriaOperator.Parse("EditStatus.Status = ?", ObjectEditStatuses.Draft)
        '                '    GenerateSequence()
        '                'End If
        '            Catch
        '                If editStautusOriginal = ObjectEditStatuses.JustCreated Then
        '                    Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.JustCreated))
        '                End If

        '                Me.UFIdHandler.CancelSequence()
        '                Throw
        '            End Try
        '        End Sub

        '        Protected Overrides Sub OnDeleted()
        '            Try
        '                MyBase.OnDeleted()
        '                Me.UFIdHandler.DeleteSequence()
        '            Catch ex As Exception
        '                Me.UFIdHandler.CancelSequence()
        '                Throw
        '            End Try
        '        End Sub


        '#End Region

    End Class

End Namespace
