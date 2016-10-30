Imports DevExpress.Xpo
Imports System.Threading
Imports DevExpress.ExpressApp
Imports DevExpress.Xpo.Metadata
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpo.DB.Exceptions
Imports DevExpress.ExpressApp.Utils
Imports DevExpress.ExpressApp.Xpo
'Imports AcurXAF.Core
'Imports AcurXAF.UFId.Core

Namespace UFId.Utils

    Public NotInheritable Class UFIdInitializer

        'Public Shared Property ObjectsWithUFIds As Dictionary(Of String, XPClassInfo)
        Public Shared Property ObjectsWithUFIds As List(Of String)



        Private Sub New()
        End Sub

        Private Shared _Application As XafApplication
        Public Shared ReadOnly Property Application() As XafApplication
            Get
                Return _Application
            End Get
        End Property
        Public Shared Sub Register(ByVal app As XafApplication)
            _Application = app
            If _Application IsNot Nothing Then
                AddHandler _Application.LoggedOn, AddressOf Application_LoggedOn
            End If
        End Sub
        Private Shared Sub Application_LoggedOn(ByVal sender As Object, ByVal e As LogonEventArgs)
            UFIdInitializer.Initialize()
        End Sub
        'Dennis: It is important to set the SequenceGenerator.DefaultDataLayer property to the main application data layer.
        'If you use a custom IObjectSpaceProvider implementation, ensure that it exposes a working IDataLayer.
        Private Shared provider As XPObjectSpaceProvider
        Public Shared Sub Initialize()
            Guard.ArgumentNotNull(UFIdInitializer.Application, "Application")
            'Dim provider As XPObjectSpaceProvider = TryCast(UFIdGeneratorInitializer.Application.ObjectSpaceProvider, XPObjectSpaceProvider)
            provider = TryCast(UFIdInitializer.Application.ObjectSpaceProvider, XPObjectSpaceProvider)
            Guard.ArgumentNotNull(provider, "provider")

            If provider.DataLayer Is Nothing Then
                'Dennis: This call is necessary to initialize a working data layer.
                'Dim objectSpace As IObjectSpace = provider.CreateObjectSpace()

                'ObjectsWithUFIds = (From q In objectSpace.GetObjects(Of UFId.DAL.UFIdConfigLink)()
                '                    Select q.TargetTypeName Distinct).ToList()
            End If

            If TypeOf provider.DataLayer Is ThreadSafeDataLayer Then
                'Dennis: We have to use a separate datalayer for the sequence generator because ThreadSafeDataLayer is usually used for ASP.NET applications.
                'SequenceGenerator.DefaultDataLayer = XpoDefault.GetDataLayer(If(Application.Connection Is Nothing, Application.ConnectionString, Application.Connection.ConnectionString), XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary, DevExpress.Xpo.DB.AutoCreateOption.None)
            Else
                'SequenceGenerator.DefaultDataLayer = provider.DataLayer
            End If
            Refresh()
        End Sub

        Public Shared Function Refresh() As List(Of String)
            Dim objectSpace As IObjectSpace = provider.CreateObjectSpace()

            ObjectsWithUFIds = (From q In objectSpace.GetObjects(Of UFId.DAL.UFIdConfigLink)()
                                Select q.TargetTypeName Distinct).ToList()
            Return ObjectsWithUFIds
        End Function
    End Class
End Namespace

