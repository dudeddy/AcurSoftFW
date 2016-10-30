Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Actions
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model
Imports DevExpress.Utils.Serializing.Helpers
Imports DevExpress.Xpo

Public Class ObjectTemplatesHelper
    Public Const Id As String = "ObjectTemplate"

    Public ReadOnly Property Controller As ViewController
    Public ReadOnly Property IsActive As Boolean

    Public ReadOnly Property ActionSaveToTemplate As SimpleAction
    Public ReadOnly Property ActionLoadFromTemplate As SingleChoiceAction

    Public ReadOnly Property ObjectSpace As IObjectSpace

    Public Sub New(viewController As ViewController, actionSaveToTemplate As SimpleAction, actionLoadFromTemplate As SingleChoiceAction)
        Me.New(viewController)
        Me.Init(actionSaveToTemplate, actionLoadFromTemplate)
    End Sub

    Public Sub New(viewController As ViewController)
        Me.Controller = viewController
        Me.ObjectSpace = viewController.GetObjectSpace()
        Me.IsActive = Me.Controller.GetModelClassEx().CanCreateObjectTemplates
        AddHandler Me.Controller.Deactivated, AddressOf Controller_Deactivated
    End Sub

    Private Sub Controller_Deactivated(sender As Object, e As EventArgs)
        SuscribeToEvents(False)
        RemoveHandler Me.Controller.Deactivated, AddressOf Controller_Deactivated
    End Sub

    Public Sub SuscribeToEvents(suscribe As Boolean)
        If Me.ActionLoadFromTemplate Is Nothing OrElse Me.ActionSaveToTemplate Is Nothing Then Return
        RemoveHandler Me.ActionSaveToTemplate.Execute, AddressOf ActionSaveToTemplate_Execute
        RemoveHandler Me.ActionLoadFromTemplate.Execute, AddressOf ActionLoadFromTemplate_Execute
        If suscribe Then
            AddHandler Me.ActionSaveToTemplate.Execute, AddressOf ActionSaveToTemplate_Execute
            AddHandler Me.ActionLoadFromTemplate.Execute, AddressOf ActionLoadFromTemplate_Execute
        End If
    End Sub

    Public Sub Init(actionSaveToTemplate As SimpleAction, actionLoadFromTemplate As SingleChoiceAction)
        _ActionSaveToTemplate = actionSaveToTemplate
        _ActionLoadFromTemplate = actionLoadFromTemplate
        Me.ActionLoadFromTemplate.Active.SetItemValue(ObjectTemplatesHelper.Id, Me.IsActive)
        Me.ActionSaveToTemplate.Active.SetItemValue(ObjectTemplatesHelper.Id, Me.IsActive)

        Me.SuscribeToEvents(Me.IsActive)

        Me.ReLoadActionLoadFromTemplateItems(Me.ObjectSpace)
    End Sub



    Public Function GetTemplateCode(
               os As IObjectSpace,
               ti As ITypeInfo,
               member As String,
               Optional tmps As IEnumerable(Of DAL.ObjectTemplate) = Nothing,
               Optional index As Nullable(Of Integer) = Nothing) As String

        If tmps Is Nothing Then
            tmps = os.GetObjects(Of DAL.ObjectTemplate).OfType(Of DAL.ObjectTemplate)
        End If
        If Not index.HasValue Then
            index = 1
        End If

        Dim newCode As String = String.Format("{0}-{1:000}", ti.Name, index.Value)
        If tmps.Any(Function(q) newCode.Equals(Convert.ToString(q.GetMemberValue(member)))) Then
            Return Me.GetTemplateCode(os, ti, member, tmps, index.Value + 1)
        End If
        Return newCode
    End Function



    Private Sub ActionSaveToTemplate_Execute(sender As Object, e As SimpleActionExecuteEventArgs)
        Dim co As XPBaseObject = DirectCast(e.CurrentObject, XPBaseObject)
        Dim os As IObjectSpace = Me.Controller.Application.CreateObjectSpace

        AddHandler os.Committed, AddressOf TemplatesObjectSpace_Committed


        Dim ti As ITypeInfo = XafTypesInfo.Instance.FindTypeInfo(co.ClassInfo.ClassType)
        Dim tmp As DAL.ObjectTemplate = os.CreateObject(Of DAL.ObjectTemplate)
        tmp.TargetTypeName = ti.FullName

        Dim code As String = ti.Name

        Dim tmps As IEnumerable(Of DAL.ObjectTemplate) = os.GetObjects(Of DAL.ObjectTemplate).OfType(Of DAL.ObjectTemplate)

        tmp.Code = GetTemplateCode(os, ti, "Code", tmps)
        tmp.Caption = GetTemplateCode(os, ti, "Caption", tmps)

        Dim modelViewItems As IModelViewItems = DirectCast(Me.Controller.View.Model, IModelDetailView).Items

        Dim qry = From q In ti.Members
                  Where Not q.IsKey AndAlso q.IsPublic AndAlso
                      q.IsPersistent AndAlso Not q.IsReadOnly AndAlso
                      q.IModelMemberAEx(Me.Controller.View.Model.Application).UseInObjectTemplates

        Dim index As Decimal = 1
        For Each m In qry
            'Me.View.Model.Application.BOModel.GetClass(Nothing).FindMember("").
            Dim templateMember As DAL.ObjectTemplateMember = os.CreateObject(Of DAL.ObjectTemplateMember)
            With templateMember
                .Template = tmp
                .MemberName = m.Name
                .MemberType = m.MemberTypeInfo.FullName
                .Index = index
                .UseKey = GetType(PersistentBase).IsAssignableFrom(m.MemberType)
                If .UseKey Then
                    Dim keyMember As IMemberInfo = m.MemberTypeInfo.KeyMember
                    .MemberValue = ObjectConverter.ObjectToString(keyMember.GetValue(m.GetValue(co)))
                Else
                    .MemberValue = ObjectConverter.ObjectToString(m.GetValue(co))
                End If
                .AllowEdit = DirectCast(modelViewItems.Item(m.Name), IModelPropertyEditor).AllowEdit
                'Me.Application.Model.BOModel.GetClass(Me.View.ObjectTypeInfo.Type).DefaultDetailView.Items.Item(m.Name)
            End With
            index += 1
        Next

        qry = From q In ti.Members
              Where Not q.IsKey AndAlso q.IsPublic AndAlso
                  q.IsList AndAlso q.IsAssociation AndAlso
                  q.IModelMemberAEx(Me.Controller.View.Model.Application).UseInObjectTemplates AndAlso
                  DirectCast(q.GetValue(co), IList).Count > 0

        For Each m In qry

            Dim templateMember As DAL.ObjectTemplateMember = os.CreateObject(Of DAL.ObjectTemplateMember)
            With templateMember
                .Template = tmp
                .MemberName = m.Name
                .MemberType = m.ListElementTypeInfo.FullName
                .Index = index
                .IsAssociation = True
                Dim child As IEnumerable(Of PersistentBase) = DirectCast(m.GetValue(co), IList).OfType(Of PersistentBase)
                .SubValuesCount = child.Count

                Dim childMembers = From q In m.ListElementTypeInfo.Members
                                   Where Not q.IsKey AndAlso q.IsPublic AndAlso
                                       Not q.IsReadOnly AndAlso q.IsPersistent AndAlso
                                       Not q.IsAssociation AndAlso
                                       q.IModelMemberAEx(Me.Controller.View.Model.Application).UseInObjectTemplates

                Dim chilModelViewItems As IModelViewItems = DirectCast(DirectCast(modelViewItems.Item(m.Name), IModelPropertyEditor).View, IModelListView).DetailView.Items

                For Each cm In childMembers
                    Dim childMember As DAL.ObjectTemplateChildMember = os.CreateObject(Of DAL.ObjectTemplateChildMember)
                    With childMember
                        .ParentMember = templateMember
                        .MemberName = cm.Name
                        .MemberType = cm.MemberTypeInfo.FullName
                        .UseKey = GetType(PersistentBase).IsAssignableFrom(cm.MemberType)

                        '.Index = index
                        Dim keyMember As IMemberInfo = Nothing
                        If .UseKey Then
                            keyMember = cm.MemberTypeInfo.KeyMember
                        End If
                        .AllowEdit = DirectCast(chilModelViewItems.Item(cm.Name), IModelPropertyEditor).AllowEdit

                        Dim id As Integer = 1
                        For Each c In child
                            Dim childMemberValue As DAL.ObjectTemplateChildMemberValue = os.CreateObject(Of DAL.ObjectTemplateChildMemberValue)
                            childMemberValue.ChildMember = childMember
                            childMemberValue.Member = childMember.ParentMember
                            If .UseKey Then
                                childMemberValue.MemberValue = ObjectConverter.ObjectToString(keyMember.GetValue(cm.GetValue(c)))
                            Else
                                childMemberValue.MemberValue = ObjectConverter.ObjectToString(cm.GetValue(c))
                            End If
                            childMemberValue.Id = id
                            id += 1
                        Next
                    End With
                Next
            End With
            index += 1
        Next

        'qry = From q In ti.OwnMembers
        '      Where Not q.IsKey AndAlso q.IsPublic AndAlso Not q.IsReadOnly AndAlso q.IsPersistent AndAlso Not q.IsList


        Dim dv As DetailView = Me.Controller.Application.CreateDetailView(os, tmp)
        e.ShowViewParameters.CreatedView = dv
        e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow
        'e.Handled = True
    End Sub


    Private Sub ActionLoadFromTemplate_Execute(sender As Object, e As SingleChoiceActionExecuteEventArgs) 'Handles ActionLoadFromTemplate.Execute
        Dim objectTemplate As DAL.ObjectTemplate = DirectCast(e.SelectedChoiceActionItem.Data, DAL.ObjectTemplate)
        Dim qry = From q In objectTemplate.ObjectTemplateMembers
                  Where Not q.IsAssociation

        Dim o As XPBaseObject = DirectCast(e.CurrentObject, XPBaseObject)

        For Each m In qry
            Dim value As Object = ObjectConverter.StringToObject(m.MemberValue, XafTypesInfo.Instance.FindTypeInfo(m.MemberType).Type)
            If m.UseKey Then
            Else
                o.SetMemberValue(m.MemberName, value)
            End If
        Next

        qry = From q In objectTemplate.ObjectTemplateMembers
              Where q.IsAssociation AndAlso q.SubValuesCount > 0

        'If qry.Count > 0 Then
        '    Dim p = qry.FirstOrDefault.
        'End If



        For Each m In qry
            Dim associatedMemberOwnerName As String = DirectCast((Me.Controller.View.ObjectTypeInfo.FindMember(m.MemberName).AssociatedMemberInfo), XafMemberInfo).AssociatedMemberOwner.Name
            For i As Integer = 1 To m.SubValuesCount
                Dim c As XPBaseObject = Me.ObjectSpace.CreateObject(XafTypesInfo.Instance.FindTypeInfo(m.MemberType).Type)
                c.SetMemberValue(associatedMemberOwnerName, o)
                Dim id As Integer = i
                Dim vals As IEnumerable(Of DAL.ObjectTemplateChildMemberValue) = m.ObjectTemplateChildMemberValues.Where(Function(q) q.Id = id)
                For Each ch In m.ObjectTemplateChildMembers
                    Dim childMemberValue As DAL.ObjectTemplateChildMemberValue = ch.ObjectTemplateChildMemberValues.FirstOrDefault(Function(q) q.Id = id)
                    Dim childValue As Object = ObjectConverter.StringToObject(childMemberValue.MemberValue, XafTypesInfo.Instance.FindTypeInfo(ch.MemberType).Type)

                    If m.UseKey Then
                    Else
                        c.SetMemberValue(ch.MemberName, childValue)
                    End If
                Next
            Next
        Next

    End Sub


    Private Sub TemplatesObjectSpace_Committed(sender As Object, e As EventArgs)
        Dim os As IObjectSpace = DirectCast(sender, IObjectSpace)
        Me.ReLoadActionLoadFromTemplateItems(os)
        RemoveHandler os.Committed, AddressOf TemplatesObjectSpace_Committed
    End Sub

    Private Sub ReLoadActionLoadFromTemplateItems(Optional os As IObjectSpace = Nothing)
        If os Is Nothing Then
            os = Me.ObjectSpace
        End If
        Me.ActionLoadFromTemplate.Items.Clear()
        Dim objectTemplates As IEnumerable(Of DAL.ObjectTemplate) = os.GetObjects(GetType(DAL.ObjectTemplate), CriteriaOperator.Parse("TargetTypeName = ?", Me.Controller.View.ObjectTypeInfo.FullName)).OfType(Of DAL.ObjectTemplate).OrderBy(Function(q) q.Caption)

        For Each ot In objectTemplates
            Me.ActionLoadFromTemplate.Items.Add(
                New ChoiceActionItem() With {
                .Data = ot,
                .Caption = ot.Caption,
                .ImageName = "LoadFromTemplate"})
        Next
    End Sub

End Class
