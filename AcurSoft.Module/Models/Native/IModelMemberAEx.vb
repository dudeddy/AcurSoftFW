Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model
Imports DevExpress.Persistent.Base
Imports System.Linq
Imports System.ComponentModel
Imports DevExpress.Xpo
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Editors
Imports AcurSoft.Numerator.Model

Namespace Model


    Public Interface IModelMemberAEx
        Inherits IModelMember
        '<ModelValueCalculator("IndexedByDefault")>
        <Category("Data")>
        <ModelReadOnly(GetType(ModelMemberNativePropertyReadOnlyCalculator))>
        Overloads Property AllowEdit As Boolean

        <Category("Format")>
        <Localizable(True)>
        <ModelReadOnly(GetType(ModelMemberNativePropertyReadOnlyCalculator))>
        <Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.MaskModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, GetType(Drawing.Design.UITypeEditor))>
        Overloads Property EditMask As String

        <Category("Format")>
        <Localizable(True)>
        <ModelReadOnly(GetType(ModelMemberNativePropertyReadOnlyCalculator))>
        Overloads Property DisplayFormat As String


        '<ModelReadOnly(GetType(ModelMemberNativePropertyReadOnlyCalculator))>
        '<Category("Format")>
        'Overloads Property EditMaskType As EditMaskType

        <Browsable(False)>
        ReadOnly Property ObjectOrderIndex As IModelObjectOrderIndex

        <Browsable(False)>
        ReadOnly Property ListOrderIndexes As IEnumerable(Of IModelListOrderIndex)
    End Interface


    <DomainLogic(GetType(IModelMemberAEx))>
    Public Class ModelMemberAExNodeLogic
        Public Shared Function Get_ObjectOrderIndex(ByVal instance As IModelMemberAEx) As IModelObjectOrderIndex
            Dim iModelClass As IModelClass = instance.ModelClass
            If Not iModelClass.TypeInfo.IsPersistent Then Return Nothing
            Dim objectOrderIndex As IModelObjectOrderIndex = instance.AppEx.OrderIndexes.ObjectOrderIndexes.GetModelObjectOrderIndex(iModelClass.TypeInfo.Type)
            If objectOrderIndex Is Nothing Then Return Nothing
            If objectOrderIndex.IndexedBy = instance.Name Then
                Return objectOrderIndex
            End If
            Return Nothing
        End Function
        Public Shared Function Get_ListOrderIndexes(ByVal instance As IModelMemberAEx) As IEnumerable(Of IModelListOrderIndex)
            Dim iModelClass As IModelClass = instance.ModelClass
            If Not iModelClass.TypeInfo.IsPersistent Then Return New List(Of IModelListOrderIndex)
            Dim qry = instance.AppEx.OrderIndexes.ListOrderIndexes.GetModelListOrderIndexesFromDetailType(iModelClass.TypeInfo.Type)
            qry = qry.Union(instance.AppEx.OrderIndexes.ListOrderIndexes.GetModelListOrderIndexesFromDetailType(iModelClass.TypeInfo.Type))
            qry = qry.Where(Function(q) q.IndexedBy = instance.Name)
            Return qry

        End Function

        'Friend Shared Function GetMemberInfoProperty(Of T)(ByVal node As IModelMember, ByVal getProperty As GetProperty(Of T)) As T
        '    Dim memberInfo As IMemberInfo = node.MemberInfo
        '    If memberInfo IsNot Nothing Then
        '        Return getProperty(memberInfo)
        '    Else
        '        Return Nothing
        '    End If
        'End Function

        Private Shared Function FindRegisteredPropertyEditor(ByVal modelMember As IModelMemberAEx) As IModelRegisteredPropertyEditor
            If (modelMember IsNot Nothing) AndAlso (modelMember.Application IsNot Nothing) Then
                Dim memberInfo As IMemberInfo = modelMember.MemberInfo
                If memberInfo IsNot Nothing Then
                    Dim fixedType As Type = If(Nullable.GetUnderlyingType(memberInfo.MemberType), memberInfo.MemberType)
                    Return modelMember.Application.ViewItems.PropertyEditors.Item(fixedType.FullName)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function Get_EditMaskDefault(ByVal instance As IModelMemberAEx) As String
            Dim result As String = String.Empty
            Dim modelRegisteredPropertyEditor As IModelRegisteredPropertyEditor = FindRegisteredPropertyEditor(instance)
            If modelRegisteredPropertyEditor IsNot Nothing Then
                result = modelRegisteredPropertyEditor.DefaultEditMask
            ElseIf instance.MemberInfo IsNot Nothing Then
                result = FormattingProvider.GetEditMask(instance.MemberInfo.MemberType)
            End If
            Return result
        End Function
        'Public Shared Function Get_EditMaskType(ByVal modelMember As IModelMember) As EditMaskType
        '    Dim result As EditMaskType = EditMaskType.Default
        '    Dim modelRegisteredPropertyEditor As IModelRegisteredPropertyEditor = FindRegisteredPropertyEditor(modelMember)
        '    If modelRegisteredPropertyEditor IsNot Nothing Then
        '        result = modelRegisteredPropertyEditor.DefaultEditMaskType
        '    ElseIf modelMember.memberInfo IsNot Nothing Then
        '        result = FormattingProvider.GetEditMaskType(modelMember.MemberInfo.MemberType)
        '    End If
        '    Return result
        'End Function

        Public Shared Function Get_DisplayFormat(ByVal instance As IModelMemberAEx) As String
            Dim objectOrderIndex As IModelObjectOrderIndex = instance.ObjectOrderIndex
            Dim listOrderIndexes As IEnumerable(Of IModelListOrderIndex) = instance.ListOrderIndexes.Where(Function(q) q.IndexedBy = instance.Name)
            Dim qry As List(Of String) = (From q In listOrderIndexes
                                          Select q.DisplayFormat Distinct).ToList()


            If objectOrderIndex IsNot Nothing AndAlso objectOrderIndex.IndexedBy = instance.Name Then
                qry.Add(objectOrderIndex.DisplayFormat)
            End If
            qry = qry.Distinct.ToList
            If qry.Count = 1 Then
                Return qry.FirstOrDefault
            End If








            Dim lookupPropertyName As String = instance.LookupProperty
            If Not String.IsNullOrEmpty(lookupPropertyName) Then
                Dim lookupMemberInfo As IMemberInfo = instance.MemberInfo.MemberTypeInfo.FindMember(lookupPropertyName)
                If lookupMemberInfo IsNot Nothing Then
                    Dim lookupPropertyClass As IModelClass = instance.Application.BOModel.GetClass(instance.Type)
                    If lookupPropertyClass IsNot Nothing Then
                        Dim lookupModelMember As IModelMember = lookupPropertyClass.OwnMembers.Item(lookupPropertyName)
                        If lookupModelMember IsNot Nothing Then
                            Return lookupModelMember.DisplayFormat
                        End If
                    End If
                End If
            End If
            Dim result As String = String.Empty
            Dim modelRegisteredPropertyEditor As IModelRegisteredPropertyEditor = FindRegisteredPropertyEditor(instance)
            If modelRegisteredPropertyEditor IsNot Nothing Then
                result = modelRegisteredPropertyEditor.DefaultDisplayFormat
            ElseIf instance.MemberInfo IsNot Nothing Then
                result = FormattingProvider.GetDisplayFormat(instance.MemberInfo.MemberType)
            End If
            Return result

        End Function


        Public Shared Function Get_EditMask(ByVal instance As IModelMemberAEx) As String
            Dim objectOrderIndex As IModelObjectOrderIndex = instance.ObjectOrderIndex
            Dim listOrderIndexes As IEnumerable(Of IModelListOrderIndex) = instance.ListOrderIndexes.Where(Function(q) q.IndexedBy = instance.Name)
            Dim qry As List(Of String) = (From q In listOrderIndexes
                                          Select q.EditMask Distinct).ToList()


            If objectOrderIndex IsNot Nothing AndAlso objectOrderIndex.IndexedBy = instance.Name Then
                qry.Add(objectOrderIndex.EditMask)
            End If
            qry = qry.Distinct.ToList
            If qry.Count = 1 Then
                Return qry.FirstOrDefault
            End If


            Dim result As String = String.Empty
            Dim modelRegisteredPropertyEditor As IModelRegisteredPropertyEditor = FindRegisteredPropertyEditor(instance)
            If modelRegisteredPropertyEditor IsNot Nothing Then
                result = modelRegisteredPropertyEditor.DefaultEditMask
            ElseIf instance.MemberInfo IsNot Nothing Then
                result = FormattingProvider.GetEditMask(instance.MemberInfo.MemberType)
            End If
            Return result
        End Function


        Public Shared Function Get_AllowEdit(ByVal instance As IModelMemberAEx) As Boolean
            Dim objectOrderIndex As IModelObjectOrderIndex = instance.ObjectOrderIndex
            Dim listOrderIndexes As IEnumerable(Of IModelListOrderIndex) = instance.ListOrderIndexes.Where(Function(q) q.IndexedBy = instance.Name)
            Dim qry As List(Of Boolean) = (From q In listOrderIndexes
                                           Select q.AllowIndexEdit Distinct).ToList()


            If objectOrderIndex IsNot Nothing AndAlso objectOrderIndex.IndexedBy = instance.Name Then
                qry.Add(objectOrderIndex.AllowIndexEdit)
            End If
            qry = qry.Distinct.ToList
            If qry.Count = 1 Then
                Return qry.FirstOrDefault
            End If

            'If objectOrderIndex Is Nothing Then
            '    If listOrderIndexes.Count = 0 Then Return True
            '    Dim values As List(Of Boolean) = listOrderIndexes.Select(Function(q) q.AllowIndexEdit).Distinct.ToList
            '    If values.Count = 1 Then Return values.FirstOrDefault
            'Else
            '    If objectOrderIndex.IndexedBy = instance.Name Then
            '        Dim value As Boolean = objectOrderIndex.AllowIndexEdit
            '        Dim values As List(Of Boolean) = listOrderIndexes.Select(Function(q) q.AllowIndexEdit).Distinct.ToList
            '        If values.Count = 0 Then
            '            Return value
            '        ElseIf values.Count = 1 AndAlso values.Any(Function(q) q = value) Then
            '            Return value
            '        End If
            '    End If
            'End If
            Return True
        End Function
    End Class


    Public Class ModelMemberNativePropertyReadOnlyCalculator
        Implements IModelIsReadOnly

        Public Function IsReadOnly(node As IModelNode, childNode As IModelNode) As Boolean Implements IModelIsReadOnly.IsReadOnly
            'Throw New NotImplementedException()
            Return False
        End Function

        Public Function IsReadOnly(node As IModelNode, propertyName As String) As Boolean Implements IModelIsReadOnly.IsReadOnly
            Dim instance As IModelMemberAEx = DirectCast(node, IModelMemberAEx)
            'If instance Then
            Dim objectOrderIndex As IModelObjectOrderIndex = instance.ObjectOrderIndex
            Dim listOrderIndexes As IEnumerable(Of IModelListOrderIndex) = instance.ListOrderIndexes.Where(Function(q) q.IndexedBy = instance.Name)

            If objectOrderIndex Is Nothing Then
                If listOrderIndexes.Count = 0 Then Return False
                Dim values As List(Of Boolean) = listOrderIndexes.Select(Function(q) q.AllowIndexEdit).Distinct.ToList
                If values.Count = 1 Then Return True
            Else
                If objectOrderIndex.IndexedBy = instance.Name Then
                    Dim value As Boolean = objectOrderIndex.AllowIndexEdit
                    If listOrderIndexes.Count = 0 Then Return True
                    Dim values As List(Of Boolean) = listOrderIndexes.Select(Function(q) q.AllowIndexEdit).Distinct.ToList
                    If values.Count = 1 AndAlso values.Any(Function(q) q = value) Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Function
    End Class
End Namespace



