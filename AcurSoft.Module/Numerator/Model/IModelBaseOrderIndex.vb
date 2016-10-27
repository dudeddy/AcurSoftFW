Imports System.ComponentModel
Imports AcurSoft.Enums
Imports AcurSoft.Numerator.Core
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model
Imports DevExpress.Persistent.Base


Namespace Numerator.Model

    Public Interface IModelBaseOrderIndex
        Inherits IModelNode, IIndexNumeratorObjectAttribute
        <Browsable(False)>
        ReadOnly Property ModelClassDataSource As IEnumerable(Of IModelClass)
        <Browsable(False)>
        ReadOnly Property IndexedByDataSource As IEnumerable(Of String)
        <Browsable(False)>
        ReadOnly Property ModelClassDataSourceNames As IEnumerable(Of String)

        <ModelReadOnly(GetType(OrderIndexPropertyReadOnlyCalculator))>
        Overloads Property Id As String

        <DataSourceProperty("IndexedByDataSource")>
        <Required>
        <Category("Data")>
        <ModelReadOnly(GetType(OrderIndexPropertyReadOnlyCalculator))>
        Overloads Property IndexedBy As String

        <DataSourceProperty("ModelClassDataSource")>
        <Category("Data")>
        ReadOnly Property ModelClass As IModelClass

        <Category("Data")>
        <DataSourceProperty("ModelClassDataSourceNames")>
        <Required>
        <ModelReadOnly(GetType(OrderIndexPropertyReadOnlyCalculator))>
        Overloads Property ObjectName As String

        <Category("Appearance")>
        ReadOnly Property ListView As IModelListView

        <Browsable(False)>
        <Category("Data")>
        ReadOnly Property IsList As Boolean

        <Category("Data")>
        ReadOnly Property Valid As Boolean

        <Category("Behavior")>
        Overloads Property AllowIndexEdit As Boolean

        <Category("Data")>
        <DefaultValue(NewItemPosition.Bottom)>
        Overloads Property NewIndexPosition As NewItemPosition

        <Category("Appearance")>
        <DefaultValue(DevExpress.Data.ColumnSortOrder.Ascending)>
        Overloads Property IndexSortOrder As DevExpress.Data.ColumnSortOrder

        <Category("Format")>
        <Localizable(True)>
        <Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.MaskModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, GetType(Drawing.Design.UITypeEditor))>
        Overloads Property EditMask As String
        '<ModelValueCalculator("""{0:C}""")>
        <Category("Format")>
        <Localizable(True)>
        Overloads Property DisplayFormat As String


        <Browsable(False)>
        ReadOnly Property IndexMember As IMemberInfo


        <Browsable(False)>
        <DefaultValue(True)>
        Overloads Property IsEditable As Boolean

    End Interface

    Public Class OrderIndexPropertyReadOnlyCalculator
        Implements IModelIsReadOnly

        Public Function IsReadOnly(node As IModelNode, childNode As IModelNode) As Boolean Implements IModelIsReadOnly.IsReadOnly
            'Throw New NotImplementedException()
            Return False
        End Function

        Public Function IsReadOnly(node As IModelNode, propertyName As String) As Boolean Implements IModelIsReadOnly.IsReadOnly
            Dim n As IModelBaseOrderIndex = DirectCast(node, IModelBaseOrderIndex)
            Return Not n.IsEditable
            'Select Case propertyName
            '    Case "ObjectName", "IndexedBy", "Id", "Member", "MasterMember", "ActiveCriteria"
            '        Return Not n.IsEditable
            'End Select
            'If propertyName <> "IndexedBy" Then Return False
            'Dim mi As XafMemberInfo = DirectCast((CType(node, IModelMember)).MemberInfo, XafMemberInfo)
            'If Not mi.IsList OrElse Not mi.IsAssociation Then Return True
            'Dim atrb As DetailIndexOrderedByAttribute = mi.FindAttribute(Of DetailIndexOrderedByAttribute)
            'If atrb Is Nothing Then Return False
            'node.SetValue(Of String)(propertyName, atrb.DetailsProprietyName)
            'If XafTypesInfo.Instance.FindTypeInfo(mi.AssociatedMemberOwner).FindMember(atrb.DetailsProprietyName) IsNot Nothing Then Return True
            Return False

        End Function
    End Class


    <DomainLogic(GetType(IModelBaseOrderIndex))>
    Public Class ModelBaseOrderIndexNodeLogic

        Public Shared Function Get_EditMask(ByVal instance As IModelBaseOrderIndex) As String
            Return "N00"
        End Function
        Public Shared Function Get_DisplayFormat(ByVal instance As IModelBaseOrderIndex) As String
            Return "{0:N00}"
        End Function
        Public Shared Function Get_ModelClass(ByVal instance As IModelBaseOrderIndex) As IModelClass
            If String.IsNullOrEmpty(instance.ObjectName) Then Return Nothing
            'instance.Application.BOModel.
            Dim typeInfo As ITypeInfo = XafTypesInfo.Instance.PersistentTypes.FirstOrDefault(Function(q) q.IsPersistent AndAlso q.Name = instance.ObjectName)
            If typeInfo Is Nothing Then Return Nothing

            Return instance.Application.BOModel.GetClass(typeInfo.Type)
        End Function

        Public Shared Function Get_ModelClassDataSource(ByVal instance As IModelBaseOrderIndex) As IEnumerable(Of IModelClass)

            Dim qry = From q In XafTypesInfo.Instance.PersistentTypes
                      Where q.IsPersistent
                      Select instance.Application.BOModel.GetClass(q.Type)

            Return qry
        End Function

        Public Shared Function Get_ModelClassDataSourceNames(ByVal instance As IModelBaseOrderIndex) As IEnumerable(Of String)

            Dim qry = From q In XafTypesInfo.Instance.PersistentTypes
                      Where q.IsPersistent
                      Select q.Name

            'If instance.ModelClass IsNot Nothing Then
            '    Dim old = From q In DirectCast(instance.Parent, IEnumerable).OfType(Of IModelBaseOrderIndex)
            '              Where q.Valid AndAlso q.Id <> instance.Id AndAlso q.ModelClass.Name = instance.ModelClass.Name
            '              Select q.ObjectName

            '    Return qry.Except(old)
            'End If

            Return qry
        End Function

        Public Shared Function Get_IsList(ByVal instance As IModelBaseOrderIndex) As Boolean
            If TypeOf instance Is IModelListOrderIndex Then Return True
            Return False
        End Function
    End Class

End Namespace

