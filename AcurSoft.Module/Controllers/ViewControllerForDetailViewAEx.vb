
Imports DevExpress.ExpressApp


Imports DevExpress.ExpressApp.Editors


Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.Xpo

Imports System
Imports System.ComponentModel
Imports System.Linq
Imports AcurSoft.Numerator.Helpers
Imports AcurSoft.Numerator
Imports AcurSoft.Numerator.Core

Namespace Controllers

    ' For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    Partial Public Class ViewControllerForDetailViewAEx
        Inherits ViewController(Of DetailView)
#Region "Numerator Properties"
        Public ReadOnly Property IndexNumeratorHelper As IndexNumeratorHelper

#End Region

        Public Sub New()
            InitializeComponent()
            ' Target required Views (via the TargetXXX properties) and create their Actions.
            'Me.TargetViewNesting = Nesting.Root
            Me.TargetViewType = ViewType.DetailView
        End Sub
#Region "Overrides"

        Protected Overrides Sub OnActivated()
            MyBase.OnActivated()
            _IndexNumeratorHelper = New IndexNumeratorHelper(Me)
            Me.Frame.GetController(Of DeleteObjectsViewController).DeleteAction.Active.SetItemValue(IndexBaseNumerator.IndexNumeratorName, Not Me.IndexNumeratorHelper.IsActive)
        End Sub

        Protected Overrides Sub OnViewControlsCreated()
            MyBase.OnViewControlsCreated()
        End Sub

        Protected Overrides Sub OnDeactivated()
            ' Unsubscribe from previously subscribed events and release other references and resources.
            MyBase.OnDeactivated()
        End Sub
    End Class
#End Region
End Namespace
