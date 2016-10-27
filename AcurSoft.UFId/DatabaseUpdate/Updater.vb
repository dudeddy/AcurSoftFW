Imports Microsoft.VisualBasic
Imports System
Imports System.Linq
Imports DevExpress.ExpressApp
Imports DevExpress.Data.Filtering
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Updating
Imports DevExpress.Xpo
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.Persistent.BaseImpl
Imports AcurSoft.UFId.DAL
Imports AcurSoft.Enums

' For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppUpdatingModuleUpdatertopic.aspx
Public Class Updater
    Inherits ModuleUpdater
    Public Sub New(ByVal objectSpace As IObjectSpace, ByVal currentDBVersion As Version)
        MyBase.New(objectSpace, currentDBVersion)
    End Sub

    Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
        MyBase.UpdateDatabaseAfterUpdateSchema()
        'Dim name As String = "MyName"
        Dim config As UFIdConfig = ObjectSpace.FindObject(Of UFIdConfig)(CriteriaOperator.Parse("Code=?", "Code1"))
        If (config Is Nothing) Then
            config = ObjectSpace.CreateObject(Of UFIdConfig)()
            With config
                .Code = "Code1"
                .Name = .Code
                .AllowReservation = True

            End With
            Dim link1 As UFIdConfigLink = ObjectSpace.CreateObject(Of UFIdConfigLink)()
            With link1
                link1.Config = config
                link1.TargetTypeName = XafTypesInfo.Instance.FindTypeInfo(GetType(TestObject)).FullName
                link1.TargetMember = "Field1"
            End With

            Dim link2 As UFIdConfigLink = ObjectSpace.CreateObject(Of UFIdConfigLink)()
            With link2
                link2.Config = config
                link2.TargetTypeName = XafTypesInfo.Instance.FindTypeInfo(GetType(TestObject)).FullName
                link2.TargetMember = "Field2"
            End With

            Dim o As TestObject = ObjectSpace.CreateObject(Of TestObject)()
            o.Tdate = Date.Today
        End If

        Dim oes As ObjectEditStatus = ObjectSpace.FindObject(Of ObjectEditStatus)(CriteriaOperator.Parse("Status=?", ObjectEditStatuses.JustCreated))
        If oes Is Nothing Then
            Dim x As Decimal = 1
            With ObjectSpace.CreateObject(Of ObjectEditStatus)
                .Code = "JustCreated"
                .Caption = "Just Created"
                .Status = ObjectEditStatuses.JustCreated
                .Index = x
            End With
            x += 1
            With ObjectSpace.CreateObject(Of ObjectEditStatus)
                .Code = "NewSaved"
                .Caption = "New Saved"
                .Status = ObjectEditStatuses.NewSaved
                .Index = x
            End With
            x += 1
            With ObjectSpace.CreateObject(Of ObjectEditStatus)
                .Code = "Draft"
                .Caption = "Draft"
                .Status = ObjectEditStatuses.Draft
                .Index = x
            End With
            x += 1
            With ObjectSpace.CreateObject(Of ObjectEditStatus)
                .Code = "Ready"
                .Caption = "Ready"
                .Status = ObjectEditStatuses.Ready
                .Index = x
            End With
            x += 1

            With ObjectSpace.CreateObject(Of ObjectEditStatus)
                .Code = "Pending"
                .Caption = "Pending"
                .Status = ObjectEditStatuses.Pending
                .Index = x
            End With
            x += 1
            With ObjectSpace.CreateObject(Of ObjectEditStatus)
                .Code = "Deleted"
                .Caption = "Deleted"
                .Status = ObjectEditStatuses.Deleted
                .Index = x
            End With
            x += 1
            With ObjectSpace.CreateObject(Of ObjectEditStatus)
                .Code = "Canceled"
                .Caption = "Canceled"
                .Status = ObjectEditStatuses.Canceled
                .Index = x
            End With
            x += 1
            With ObjectSpace.CreateObject(Of ObjectEditStatus)
                .Code = "Locked"
                .Caption = "Locked"
                .Status = ObjectEditStatuses.Locked
                .Index = x
            End With
            x += 1
            With ObjectSpace.CreateObject(Of ObjectEditStatus)
                .Code = "Other"
                .Caption = "Other"
                .Status = ObjectEditStatuses.Other
                .Index = x
            End With
            x += 1
        End If
        ObjectSpace.CommitChanges() 'Uncomment this line to persist created object(s).
    End Sub

    Public Overrides Sub UpdateDatabaseBeforeUpdateSchema()
        MyBase.UpdateDatabaseBeforeUpdateSchema()
        'If (CurrentDBVersion < New Version("1.1.0.0") AndAlso CurrentDBVersion > New Version("0.0.0.0")) Then
        '    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName")
        'End If
    End Sub
End Class