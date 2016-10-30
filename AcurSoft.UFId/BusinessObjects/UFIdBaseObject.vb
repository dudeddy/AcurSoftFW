Imports System.ComponentModel
Imports AcurSoft.Enums
Imports AcurSoft.UFId.Utils
Imports DevExpress.Data.Filtering
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Xpo

Namespace UFId.DAL

    <NonPersistent>
    Public MustInherit Class UFIdBaseObject
        Inherits BaseObject
        Private _UFIdHelper As UFIdHelper
        <Browsable(False)>
        Public ReadOnly Property UFIdHelper As UFIdHelper
            Get
                If _UFIdHelper Is Nothing Then
                    _UFIdHelper = New UFIdHelper(Me)
                End If
                Return _UFIdHelper
            End Get
        End Property

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
            Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.JustCreated))
            Me.UFIdHelper.SetDraftSequenceValues()
        End Sub

        Dim _EditStatus As ObjectEditStatus
        Public Property EditStatus() As ObjectEditStatus
            Get
                Return _EditStatus
            End Get
            Set(ByVal value As ObjectEditStatus)
                SetPropertyValue(Of ObjectEditStatus)(NameOf(EditStatus), _EditStatus, value)
            End Set
        End Property


#Region "Overrides"

        Protected Overrides Sub OnSaving()
            Dim editStautusOriginal As ObjectEditStatuses = Me.EditStatus.Status
            MyBase.OnSaving()
            If editStautusOriginal = ObjectEditStatuses.JustCreated Then
                Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.NewSaved))
            End If
            Try
                MyBase.OnSaving()
                If editStautusOriginal = ObjectEditStatuses.JustCreated Then
                    Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.NewSaved))
                End If
                'If Not (TypeOf Session Is NestedUnitOfWork) AndAlso (Session.DataLayer IsNot Nothing) AndAlso (TypeOf Session.ObjectLayer Is SimpleObjectLayer) AndAlso Session.IsNewObject(Me) Then
                Me.UFIdHelper.TrySave()
                'If Not (TypeOf Session Is NestedUnitOfWork) AndAlso (Session.DataLayer IsNot Nothing) AndAlso (TypeOf Session.ObjectLayer Is SimpleObjectLayer) Then
                '    'OR
                '    '&& !(Session.ObjectLayer is DevExpress.ExpressApp.Security.ClientServer.SecuredSessionObjectLayer)
                '    'Dim co As CriteriaOperator = CriteriaOperator.Parse("EditStatus.Status = ?", ObjectEditStatuses.Draft)
                '    GenerateSequence()
                'End If
            Catch
                If editStautusOriginal = ObjectEditStatuses.JustCreated Then
                    Me.EditStatus = Me.Session.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.JustCreated))
                End If

                Me.UFIdHelper.CancelSequence()
                Throw
            End Try
        End Sub

        Protected Overrides Sub OnDeleted()
            Try
                MyBase.OnDeleted()
                Me.UFIdHelper.DeleteSequence()
            Catch ex As Exception
                Me.UFIdHelper.CancelSequence()
                Throw
            End Try
        End Sub
#End Region
    End Class
End Namespace
