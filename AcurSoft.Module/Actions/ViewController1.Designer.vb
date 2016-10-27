Partial Class ViewController1

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New(ByVal Container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        Container.Add(Me)

    End Sub

    'Component overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.CheckAction1 = New AcurSoft.Actions.CheckAction(Me.components)
        '
        'CheckAction1
        '
        Me.CheckAction1.Caption = "1"
        Me.CheckAction1.CaptionChecked = "1"
        Me.CheckAction1.CaptionUnChecked = "0"
        Me.CheckAction1.Checked = True
        Me.CheckAction1.ConfirmationMessage = Nothing
        Me.CheckAction1.Id = "94813786-f802-4c70-862e-0f10b2129f8b"
        Me.CheckAction1.ImageNameChecked = "State_Priority_Low"
        Me.CheckAction1.ImageNameUnChecked = "State_Priority_High"
        Me.CheckAction1.TargetObjectsCriteriaMode = DevExpress.ExpressApp.Actions.TargetObjectsCriteriaMode.TrueForAll
        Me.CheckAction1.ToolTip = Nothing
        '
        'ViewController1
        '
        Me.Actions.Add(Me.CheckAction1)

    End Sub

    Friend WithEvents CheckAction1 As AcurSoft.Actions.CheckAction
End Class
