Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports System.Linq
Imports DevExpress.ExpressApp
Imports System.ComponentModel
Imports DevExpress.ExpressApp.DC
Imports System.Collections.Generic
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.Actions
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.ExpressApp.Updating
Imports DevExpress.ExpressApp.Model.Core
Imports DevExpress.ExpressApp.Model.DomainLogics
Imports DevExpress.ExpressApp.Model.NodeGenerators
Imports DevExpress.ExpressApp.Xpo
Imports AcurSoft.Model
Imports AcurSoft.Helpers
Imports AcurSoft.Enums

' For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppModuleBasetopic.aspx.
Public NotInheritable Class ModuleCore
    Inherits ModuleBase



    Private Shared _ApplicationPlatform As PlatformKind
    Public Shared ReadOnly Property ApplicationPlatform As PlatformKind
        Get
            _ApplicationPlatform = PlatformKind.Unknown
            If XApplication.MainWindow.GetType.ToString.Contains(".Win.") Then
                _ApplicationPlatform = PlatformKind.Win
            ElseIf XApplication.MainWindow.GetType.ToString.Contains(".Web.") Then
                _ApplicationPlatform = PlatformKind.Web

            End If
            Return _ApplicationPlatform
        End Get
    End Property
    Public Sub New()
        InitializeComponent()
        BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction
    End Sub

    Public Overrides Function GetModuleUpdaters(ByVal objectSpace As IObjectSpace, ByVal versionFromDB As Version) As IEnumerable(Of ModuleUpdater)
        Dim updater As ModuleUpdater = New Updater(objectSpace, versionFromDB)
        Return New ModuleUpdater() {updater}
    End Function
    Public Shared Property XApplication As XafApplication
    Public Overrides Sub Setup(application As XafApplication)
        MyBase.Setup(application)
        XApplication = application
        ' Manage various aspects of the application UI and behavior at the module level.
        'AddHandler application.ModelChanged, AddressOf application_ModelChanged
        'AddHandler application.SettingUp, AddressOf application_SettingUp
        AddHandler application.SetupComplete, AddressOf application_SetupComplete

        'AddHandler XApplication.ListViewCreated, AddressOf Application_ListViewCreated
        'AddHandler XApplication.DetailViewCreated, AddressOf Application_DetailViewCreated
    End Sub

    'Private Sub Application_DetailViewCreated(sender As Object, e As DetailViewCreatedEventArgs)
    '    Dim ee = e
    'End Sub

    'Private Sub Application_ListViewCreated(sender As Object, e As ListViewCreatedEventArgs)
    '    'Throw New NotImplementedException()
    '    AddHandler e.ListView.CreateCustomCurrentObjectDetailView, AddressOf ListView_CreateCustomCurrentObjectDetailView
    '    'AddHandler e.ListView.CreateCustomCurrentObjectDetailView, New EventHandler(Of CreateCustomCurrentObjectDetailViewEventArgs)
    '    'If e.ListView.Id = "DevExpress.Persistent.BaseImpl.Person_ListView" Then
    '    'End If
    'End Sub

    'Private Sub ListView_CreateCustomCurrentObjectDetailView(sender As Object, e As CreateCustomCurrentObjectDetailViewEventArgs)
    '    Dim ee = e
    'End Sub

    Private Sub application_SetupComplete(sender As Object, e As EventArgs)

    End Sub

    'Private Sub application_SettingUp(sender As Object, e As SetupEventArgs)
    '    Dim x = Me.Application.Model
    '    Dim z = 0
    '    'Throw New NotImplementedException()
    'End Sub

    'Private Sub application_ModelChanged(sender As Object, e As EventArgs)
    '    Dim x = 0
    '    'Throw New NotImplementedException()
    'End Sub

    Public Overrides Sub CustomizeTypesInfo(ByVal typesInfo As ITypesInfo)
        MyBase.CustomizeTypesInfo(typesInfo)
        CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo)
    End Sub

    Public Overrides Sub AddGeneratorUpdaters(ByVal updaters As ModelNodesGeneratorUpdaters)
        MyBase.AddGeneratorUpdaters(updaters)
        'updaters.Add(New ViewsNodesGeneratorUpdater())
    End Sub
    Public Overrides Sub ExtendModelInterfaces(ByVal extenders As ModelInterfaceExtenders)
        MyBase.ExtendModelInterfaces(extenders)
        extenders.Add(Of IModelApplication, IModelApplicationAEx)()
        extenders.Add(Of IModelClass, IModelClassAEx)()
        extenders.Add(Of IModelMember, IModelMemberAEx)()

        'extenders.Add(Of IModelDetailView, IModelDetailViewEx)()
        'extenders.Add(Of IModelListView, IModelListViewEx)()
        'XafTypesInfo.Instance.PersistentTypes

        Dim xx = 0
    End Sub

    'Overrides  
End Class
