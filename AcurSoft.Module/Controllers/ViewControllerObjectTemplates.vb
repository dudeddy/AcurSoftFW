Imports Microsoft.VisualBasic
Imports System
Imports System.Linq
Imports System.Text
Imports DevExpress.ExpressApp
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Utils
Imports DevExpress.ExpressApp.Layout
Imports DevExpress.ExpressApp.Actions
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.ExpressApp.Templates
Imports DevExpress.Persistent.Validation
Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.ExpressApp.Model.NodeGenerators
Imports DevExpress.Xpo
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Utils.Serializing.Helpers
Imports DevExpress.ExpressApp.Model

' For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
Partial Public Class ViewControllerObjectTemplates
    Inherits ViewController


    Public ReadOnly Property ActionSaveToTemplate As SimpleAction
    Public ReadOnly Property ActionLoadFromTemplate As SingleChoiceAction

    Public ReadOnly Property ObjectTemplatesHelper As ObjectTemplatesHelper

    Public Sub New()
        InitializeComponent()
        ' Target required Views (via the TargetXXX properties) and create their Actions.
        Me.InitObjectTemplatesActions()
        Me.TargetViewType = ViewType.DetailView
    End Sub

    Public Sub InitObjectTemplatesActions()
        '
        'ActionSaveToTemplate
        '
        _ActionSaveToTemplate = New SimpleAction(Me.components)
        With Me.ActionSaveToTemplate
            .Caption = "Save To Template"
            .Category = "Save"
            .Id = "SaveToTemplate"
            .ToolTip = "Save To Template"
            .ImageName = "SaveAsTemplate"
        End With
        Me.Actions.Add(Me.ActionSaveToTemplate)
        '
        'ActionLoadFromTemplate
        '
        _ActionLoadFromTemplate = New SingleChoiceAction(Me.components)
        With Me.ActionLoadFromTemplate
            .Caption = "Load From Template"
            .Category = "RecordEdit"
            .Id = "LoadFromTemplate"
            .ItemType = SingleChoiceActionItemType.ItemIsOperation
            .ToolTip = "Load From Template"
            .ImageName = "LoadFromTemplate"
        End With
        Me.Actions.Add(Me.ActionLoadFromTemplate)
        '
        'ViewController2
        '
    End Sub

    Protected Overrides Sub OnActivated()
        MyBase.OnActivated()
        ' Perform various tasks depending on the target View.
        _ObjectTemplatesHelper = New ObjectTemplatesHelper(Me, Me.ActionSaveToTemplate, Me.ActionLoadFromTemplate)
        'Me.ActionLoadFromTemplate.Active.SetItemValue(ObjectTemplatesHelper.Id, Me.ObjectTemplatesHelper.IsActive)
        'Me.ActionSaveToTemplate.Active.SetItemValue(ObjectTemplatesHelper.Id, Me.ObjectTemplatesHelper.IsActive)

        'If Me.ObjectTemplatesHelper.IsActive Then
        '    Me.ReLoadActionLoadFromTemplateItems()

        'End If
    End Sub

    'Private Sub ReLoadActionLoadFromTemplateItems(Optional os As IObjectSpace = Nothing)
    '    If os Is Nothing Then
    '        os = Me.ObjectSpace
    '    End If
    '    Me.ActionLoadFromTemplate.Items.Clear()
    '    Dim objectTemplates As IEnumerable(Of DAL.ObjectTemplate) = os.GetObjects(GetType(DAL.ObjectTemplate), CriteriaOperator.Parse("TargetTypeName = ?", Me.View.ObjectTypeInfo.FullName)).OfType(Of DAL.ObjectTemplate).OrderBy(Function(q) q.Caption)

    '    For Each ot In objectTemplates
    '        Me.ActionLoadFromTemplate.Items.Add(
    '            New ChoiceActionItem() With {
    '            .Data = ot,
    '            .Caption = ot.Caption})
    '    Next
    'End Sub

    Protected Overrides Sub OnViewControlsCreated()
        MyBase.OnViewControlsCreated()
        ' Access and customize the target View control.
    End Sub
    Protected Overrides Sub OnDeactivated()
        ' Unsubscribe from previously subscribed events and release other references and resources.
        MyBase.OnDeactivated()
    End Sub

    'Public Function GetTemplateCode(
    '           os As IObjectSpace,
    '           ti As ITypeInfo,
    '           member As String,
    '           Optional tmps As IEnumerable(Of DAL.ObjectTemplate) = Nothing,
    '           Optional index As Nullable(Of Integer) = Nothing) As String

    '    If tmps Is Nothing Then
    '        tmps = os.GetObjects(Of DAL.ObjectTemplate).OfType(Of DAL.ObjectTemplate)
    '    End If
    '    If Not index.HasValue Then
    '        index = 1
    '    End If

    '    Dim newCode As String = String.Format("{0}-{1:000}", ti.Name, index.Value)
    '    If tmps.Any(Function(q) newCode.Equals(Convert.ToString(q.GetMemberValue(member)))) Then
    '        Return Me.GetTemplateCode(os, ti, member, tmps, index.Value + 1)
    '    End If
    '    Return newCode
    'End Function


    'Private Sub ActionSaveToTemplate_Execute(sender As Object, e As SimpleActionExecuteEventArgs)
    '    Dim co As XPBaseObject = DirectCast(e.CurrentObject, XPBaseObject)
    '    Dim os As IObjectSpace = Me.Application.CreateObjectSpace

    '    AddHandler os.Committed, AddressOf TemplatesObjectSpace_Committed


    '    Dim ti As ITypeInfo = XafTypesInfo.Instance.FindTypeInfo(co.ClassInfo.ClassType)
    '    Dim tmp As DAL.ObjectTemplate = os.CreateObject(Of DAL.ObjectTemplate)
    '    tmp.TargetTypeName = ti.FullName

    '    Dim code As String = ti.Name

    '    Dim tmps As IEnumerable(Of DAL.ObjectTemplate) = os.GetObjects(Of DAL.ObjectTemplate).OfType(Of DAL.ObjectTemplate)

    '    tmp.Code = GetTemplateCode(os, ti, "Code", tmps)
    '    tmp.Caption = GetTemplateCode(os, ti, "Caption", tmps)

    '    Dim modelViewItems As IModelViewItems = DirectCast(Me.View.Model, IModelDetailView).Items

    '    Dim qry = From q In ti.Members
    '              Where Not q.IsKey AndAlso q.IsPublic AndAlso
    '                  q.IsPersistent AndAlso Not q.IsReadOnly AndAlso
    '                  q.IModelMemberAEx(Me.View.Model.Application).UseInObjectTemplates

    '    Dim index As Decimal = 1
    '    For Each m In qry
    '        'Me.View.Model.Application.BOModel.GetClass(Nothing).FindMember("").
    '        Dim templateMember As DAL.ObjectTemplateMember = os.CreateObject(Of DAL.ObjectTemplateMember)
    '        With templateMember
    '            .Template = tmp
    '            .MemberName = m.Name
    '            .MemberType = m.MemberTypeInfo.FullName
    '            .Index = index
    '            .UseKey = GetType(PersistentBase).IsAssignableFrom(m.MemberType)
    '            If .UseKey Then
    '                Dim keyMember As IMemberInfo = m.MemberTypeInfo.KeyMember
    '                .MemberValue = ObjectConverter.ObjectToString(keyMember.GetValue(m.GetValue(co)))
    '            Else
    '                .MemberValue = ObjectConverter.ObjectToString(m.GetValue(co))
    '            End If
    '            .AllowEdit = DirectCast(modelViewItems.Item(m.Name), IModelPropertyEditor).AllowEdit
    '            'Me.Application.Model.BOModel.GetClass(Me.View.ObjectTypeInfo.Type).DefaultDetailView.Items.Item(m.Name)
    '        End With
    '        index += 1
    '    Next

    '    qry = From q In ti.Members
    '          Where Not q.IsKey AndAlso q.IsPublic AndAlso
    '              q.IsList AndAlso q.IsAssociation AndAlso
    '              q.IModelMemberAEx(Me.View.Model.Application).UseInObjectTemplates AndAlso
    '              DirectCast(q.GetValue(co), IList).Count > 0

    '    For Each m In qry

    '        Dim templateMember As DAL.ObjectTemplateMember = os.CreateObject(Of DAL.ObjectTemplateMember)
    '        With templateMember
    '            .Template = tmp
    '            .MemberName = m.Name
    '            .MemberType = m.ListElementTypeInfo.FullName
    '            .Index = index
    '            .IsAssociation = True
    '            Dim child As IEnumerable(Of PersistentBase) = DirectCast(m.GetValue(co), IList).OfType(Of PersistentBase)
    '            .SubValuesCount = child.Count

    '            Dim childMembers = From q In m.ListElementTypeInfo.Members
    '                               Where Not q.IsKey AndAlso q.IsPublic AndAlso
    '                                   Not q.IsReadOnly AndAlso q.IsPersistent AndAlso
    '                                   Not q.IsAssociation AndAlso
    '                                   q.IModelMemberAEx(Me.View.Model.Application).UseInObjectTemplates

    '            Dim chilModelViewItems As IModelViewItems = DirectCast(DirectCast(modelViewItems.Item(m.Name), IModelPropertyEditor).View, IModelListView).DetailView.Items

    '            For Each cm In childMembers
    '                Dim childMember As DAL.ObjectTemplateChildMember = os.CreateObject(Of DAL.ObjectTemplateChildMember)
    '                With childMember
    '                    .ParentMember = templateMember
    '                    .MemberName = cm.Name
    '                    .MemberType = cm.MemberTypeInfo.FullName
    '                    .UseKey = GetType(PersistentBase).IsAssignableFrom(cm.MemberType)

    '                    '.Index = index
    '                    Dim keyMember As IMemberInfo = Nothing
    '                    If .UseKey Then
    '                        keyMember = cm.MemberTypeInfo.KeyMember
    '                    End If
    '                    .AllowEdit = DirectCast(chilModelViewItems.Item(cm.Name), IModelPropertyEditor).AllowEdit

    '                    Dim id As Integer = 1
    '                    For Each c In child
    '                        Dim childMemberValue As DAL.ObjectTemplateChildMemberValue = os.CreateObject(Of DAL.ObjectTemplateChildMemberValue)
    '                        childMemberValue.ChildMember = childMember
    '                        childMemberValue.Member = childMember.ParentMember
    '                        If .UseKey Then
    '                            childMemberValue.MemberValue = ObjectConverter.ObjectToString(keyMember.GetValue(cm.GetValue(c)))
    '                        Else
    '                            childMemberValue.MemberValue = ObjectConverter.ObjectToString(cm.GetValue(c))
    '                        End If
    '                        childMemberValue.Id = id
    '                        id += 1
    '                    Next
    '                End With
    '            Next
    '        End With
    '        index += 1
    '    Next

    '    'qry = From q In ti.OwnMembers
    '    '      Where Not q.IsKey AndAlso q.IsPublic AndAlso Not q.IsReadOnly AndAlso q.IsPersistent AndAlso Not q.IsList


    '    Dim dv As DetailView = Me.Application.CreateDetailView(os, tmp)
    '    e.ShowViewParameters.CreatedView = dv
    '    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow
    '    'e.Handled = True
    'End Sub

    'Private Sub TemplatesObjectSpace_Committed(sender As Object, e As EventArgs)
    '    Dim os As IObjectSpace = DirectCast(sender, IObjectSpace)
    '    Me.ReLoadActionLoadFromTemplateItems(os)
    '    RemoveHandler os.Committed, AddressOf TemplatesObjectSpace_Committed
    'End Sub

    'Private Sub ActionLoadFromTemplate_Execute(sender As Object, e As SingleChoiceActionExecuteEventArgs) ' Handles ActionLoadFromTemplate.Execute
    '    Dim objectTemplate As DAL.ObjectTemplate = DirectCast(e.SelectedChoiceActionItem.Data, DAL.ObjectTemplate)
    '    Dim qry = From q In objectTemplate.ObjectTemplateMembers
    '              Where Not q.IsAssociation

    '    Dim o As XPBaseObject = DirectCast(e.CurrentObject, XPBaseObject)

    '    For Each m In qry
    '        Dim value As Object = ObjectConverter.StringToObject(m.MemberValue, XafTypesInfo.Instance.FindTypeInfo(m.MemberType).Type)
    '        If m.UseKey Then
    '        Else
    '            o.SetMemberValue(m.MemberName, value)
    '        End If
    '    Next

    '    qry = From q In objectTemplate.ObjectTemplateMembers
    '          Where q.IsAssociation AndAlso q.SubValuesCount > 0

    '    'If qry.Count > 0 Then
    '    '    Dim p = qry.FirstOrDefault.
    '    'End If



    '    For Each m In qry
    '        Dim associatedMemberOwnerName As String = DirectCast((Me.View.ObjectTypeInfo.FindMember(m.MemberName).AssociatedMemberInfo), XafMemberInfo).AssociatedMemberOwner.Name
    '        For i As Integer = 1 To m.SubValuesCount
    '            Dim c As XPBaseObject = Me.ObjectSpace.CreateObject(XafTypesInfo.Instance.FindTypeInfo(m.MemberType).Type)
    '            c.SetMemberValue(associatedMemberOwnerName, o)
    '            Dim id As Integer = i
    '            Dim vals As IEnumerable(Of DAL.ObjectTemplateChildMemberValue) = m.ObjectTemplateChildMemberValues.Where(Function(q) q.Id = id)
    '            For Each ch In m.ObjectTemplateChildMembers
    '                Dim childMemberValue As DAL.ObjectTemplateChildMemberValue = ch.ObjectTemplateChildMemberValues.FirstOrDefault(Function(q) q.Id = id)
    '                Dim childValue As Object = ObjectConverter.StringToObject(childMemberValue.MemberValue, XafTypesInfo.Instance.FindTypeInfo(ch.MemberType).Type)

    '                If m.UseKey Then
    '                Else
    '                    c.SetMemberValue(ch.MemberName, childValue)
    '                End If
    '            Next
    '        Next
    '    Next

    'End Sub
End Class
