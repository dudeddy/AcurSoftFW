Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Actions
Imports DevExpress.Persistent.Base
Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Threading
'Imports Xafari

Namespace Actions
    '<ToolboxBitmap(GetType(XafariModule), "Bitmaps256.CheckAction.ico"), ToolboxItem(True)>
    <ToolboxItem(True)>
    Public Class CheckAction
        Inherits SimpleAction

        Private _Checked As Boolean
        Public Event CheckedChanged As EventHandler(Of ActionCheckEventArgs)

        Public Event CheckedChanging As EventHandler(Of ActionCheckCancelEventArgs)
        'Private _Checking As Boolean
        Public Property Checked() As Boolean
            Get
                Return _Checked
            End Get
            Set(ByVal value As Boolean)
                'If Not _IsActivated Then Return
                If _Checked <> value Then
                    Dim cancel As Boolean = False
                    Dim onCheckedChangingArg As New ActionCheckCancelEventArgs(cancel, value)
                    'Dim a = _IsActivated
                    Me.OnCheckedChanging(onCheckedChangingArg)
                    If Not onCheckedChangingArg.Cancel Then
                        _Checked = onCheckedChangingArg.Checked
                        Me.OnCheckedChanged(New ActionCheckEventArgs(Me))
                    End If
                End If
            End Set
        End Property

        Private _OrignalCaption As String
        Private _OrignalImageName As String


        Private _CaptionChecked As String
        <DefaultValue("On")>
        Public Property CaptionChecked As String
            Get
                Return _CaptionChecked
            End Get
            Set(value As String)
                If value <> _CaptionChecked Then
                    'Me.OnChanged(New ActionChangedEventArgs(ActionChangedType.Image))

                    If Me.Checked Then
                        Me.Caption = value
                    End If
                    _CaptionChecked = value
                End If
            End Set
        End Property

        Private _CaptionUnChecked As String
        <DefaultValue("Off")>
        Public Property CaptionUnChecked As String
            Get
                Return _CaptionUnChecked
            End Get
            Set(value As String)
                If value <> _CaptionUnChecked Then
                    If Not Me.Checked Then
                        Me.Caption = value
                    End If
                    _CaptionUnChecked = value
                End If
            End Set
        End Property


        Private _ImageNameChecked As String
        Public Property ImageNameChecked As String
            Get
                Return _ImageNameChecked
            End Get
            Set(value As String)
                If value <> _ImageNameChecked Then
                    'Me.OnChanged(New ActionChangedEventArgs(ActionChangedType.Image))

                    If Me.Checked Then
                        Me.ImageName = value
                    End If
                    _ImageNameChecked = value
                End If
            End Set
        End Property

        Private _ImageNameUnChecked As String
        Public Property ImageNameUnChecked As String
            Get
                Return _ImageNameUnChecked
            End Get
            Set(value As String)
                If value <> _ImageNameUnChecked Then
                    If Not Me.Checked Then
                        Me.ImageName = value
                    End If
                    _ImageNameUnChecked = value
                End If
            End Set
        End Property

        Protected Overrides Sub OnExecuting(e As CancelEventArgs)
            MyBase.OnExecuting(e)
        End Sub

        Protected Overrides Sub OnExecuted(e As ActionBaseEventArgs)
            MyBase.OnExecuted(e)
            Me.Checked = Not Me.Checked
        End Sub

        Public Sub New()
        End Sub

        Private _OrignalImageNameInited As Boolean
        Public Overloads Property ImageName As String
            Get
                Return MyBase.ImageName
            End Get
            Set(value As String)
                If Not _OrignalImageNameInited Then
                    _OrignalImageName = MyBase.ImageName
                    _OrignalImageNameInited = True
                End If
                MyBase.ImageName = value
            End Set
        End Property

        Private _OrignalCaptionInited As Boolean
        Public Overloads Property Caption As String
            Get
                Return MyBase.Caption
            End Get
            Set(value As String)
                If Not _OrignalCaptionInited Then
                    _OrignalCaption = MyBase.Caption
                    _OrignalCaptionInited = True
                End If
                MyBase.Caption = value
            End Set
        End Property

        'Private _IsActivated As Boolean
        Protected Overrides Sub OnActivated()
            '_IsActivated = True
            MyBase.OnActivated()
            If _OrignalCaption Is Nothing Then
                _OrignalCaption = Me.Caption
            End If
            If _OrignalImageName Is Nothing Then
                _OrignalCaption = Me.ImageName
            End If
        End Sub

        Public Sub New(ByVal container As IContainer)
            MyBase.New(container)
        End Sub

        Public Sub New(ByVal owner As Controller, ByVal id As String, ByVal category As PredefinedCategory)
            Me.New(owner, id, category.ToString())
        End Sub

        Public Sub New(ByVal owner As Controller, ByVal id As String, ByVal category As String)
            MyBase.New(owner, id, category)
        End Sub

        Protected Sub OnCheckedChanged(ByVal e As ActionCheckEventArgs)
            If _Checked AndAlso Not String.IsNullOrEmpty(Me.CaptionChecked) Then
                Me.Caption = Me.CaptionChecked
            ElseIf Not _Checked AndAlso Not String.IsNullOrEmpty(Me.CaptionUnChecked) Then
                Me.Caption = Me.CaptionUnChecked
            Else
                Me.Caption = _OrignalCaption
            End If
            If _Checked AndAlso Not String.IsNullOrEmpty(Me.ImageNameChecked) Then
                Me.ImageName = Me.ImageNameChecked
            ElseIf Not _Checked AndAlso Not String.IsNullOrEmpty(Me.ImageNameUnChecked) Then
                Me.ImageName = Me.ImageNameUnChecked
            Else
                Me.ImageName = _OrignalImageName
            End If
            RaiseEvent CheckedChanged(Me, e)
        End Sub

        Protected Sub OnCheckedChanging(ByVal e As ActionCheckCancelEventArgs)
            'Me.OnExecuting(e)
            RaiseEvent CheckedChanging(Me, e)
            'If Me.CheckedChanging IsNot Nothing Then
            'End If
        End Sub

    End Class
    Public Class ActionCheckCancelEventArgs
        Inherits CancelEventArgs
        'Public ReadOnly Property Action As CheckAction
        Public Property Checked As Boolean
        'Public Sub New()
        '    MyBase.New()
        'End Sub
        'Public Sub New(cancel As Boolean)
        '    MyBase.New(cancel)
        'End Sub
        Public Sub New(cancel As Boolean, checked As Boolean)
            MyBase.New(cancel)
            Me.Checked = checked
        End Sub
    End Class

    Public Class ActionCheckEventArgs
        Inherits ActionBaseEventArgs
        Public Overloads ReadOnly Property Action As CheckAction
        Public ReadOnly Property Checked As Boolean
            Get
                Return Me.Action.Checked
            End Get
        End Property
        Public Sub New(action As CheckAction)
            MyBase.New(action)
        End Sub
    End Class
End Namespace
