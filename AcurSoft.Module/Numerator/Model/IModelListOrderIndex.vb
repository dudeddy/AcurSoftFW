Imports System.ComponentModel
Imports AcurSoft.Core
Imports AcurSoft.Numerator.Core
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model
Imports DevExpress.Persistent.Base
Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata

Namespace Numerator.Model

    <KeyProperty("Id")>
    Public Interface IModelListOrderIndex
        Inherits IModelNode, IModelBaseOrderIndex, IIndexNumeratorListAttribute

        <Category("Data")>
        <ModelReadOnly(GetType(OrderIndexPropertyReadOnlyCalculator))>
        Overloads Property ActiveCriteria As String

        <Browsable(False)>
        <Category("Data")>
        ReadOnly Property ActiveCriteriaOperator As CriteriaOperator


        <Category("Data")>
        <DataSourceProperty("ModelClassDataSource")>
        ReadOnly Property DetailModelClass As IModelClass

        <Category("Data")>
        <DataSourceProperty("MasterMemberDataSource")>
        <Required>
        <ModelReadOnly(GetType(OrderIndexPropertyReadOnlyCalculator))>
        Overloads Property MasterMember As String
        <Browsable(False)>
        ReadOnly Property MasterMemberDataSource As IEnumerable(Of String)

        <Category("Data")>
        <DataSourceProperty("MemberDataSource")>
        <Required>
        <ModelReadOnly(GetType(OrderIndexPropertyReadOnlyCalculator))>
        Overloads Property Member As String
        <Browsable(False)>
        ReadOnly Property MemberDataSource As IEnumerable(Of String)


        <Category("Appearance")>
        ReadOnly Property MemberDetailView As IModelDetailView

        <Browsable(False)>
        ReadOnly Property ValidActiveCriteriaOperator As XNullable(Of CriteriaOperator)

    End Interface

    <DomainLogic(GetType(IModelListOrderIndex))>
    Public Class ModelListOrderIndexNodeLogic


        Public Shared Function Get_ActiveCriteria(ByVal instance As IModelListOrderIndex) As String
            If Not instance.Valid Then Return Nothing
            'Master_Details_ListView
            Return String.Format("[{0}] Is Not Null", instance.MasterMember)

        End Function

        Public Shared Function Get_ActiveCriteriaOperator(ByVal instance As IModelListOrderIndex) As CriteriaOperator
            If Not instance.Valid Then Return Nothing
            If String.IsNullOrEmpty(instance.ActiveCriteria) Then Return Nothing
            Return CriteriaOperator.TryParse(instance.ActiveCriteria)
        End Function

        Public Shared Function Get_DetailModelClass(ByVal instance As IModelListOrderIndex) As IModelClass
            If instance.ModelClass Is Nothing Then Return Nothing
            If String.IsNullOrEmpty(instance.Member) Then Return Nothing
            Dim member As IModelMember = instance.ModelClass.OwnMembers.FirstOrDefault(Function(q) q.Name = instance.Member)
            If member Is Nothing Then Return Nothing
            Return instance.Application.BOModel.GetClass(member.MemberInfo.ListElementTypeInfo.Type)
        End Function

        Public Shared Function Get_IndexedByDataSource(ByVal instance As IModelListOrderIndex) As IEnumerable(Of String)
            If instance.DetailModelClass Is Nothing Then Return New List(Of String)

            Dim qry = From q In instance.DetailModelClass.OwnMembers
                      Where q.MemberInfo.MemberType.IsNumericType()
                      Select q.Name

            Dim parent = DirectCast(instance.Parent, IModelListOrderIndexes)
            If parent.LongCount(Function(q) q.Id <> instance.Id AndAlso instance.ObjectName.Equals(q.ObjectName) AndAlso instance.Member.Equals(q.Member)) > 0 Then
                Return New List(Of String)
            End If

            'Dim old = From q In DirectCast(instance.Parent, IModelListOrderIndexes).OfType(Of IModelListOrderIndex)
            '          Where q.ModelClass.Name = instance.ModelClass.Name AndAlso q.Member = instance.Member AndAlso q.Id <> instance.Id
            '          Select q.IndexedBy

            'Return qry.Except(old).ToList()
            Return qry.ToList()
        End Function

        Public Shared Function Get_IndexMember(ByVal instance As IModelListOrderIndex) As IMemberInfo
            If Not instance.Valid Then Return Nothing
            Return instance.DetailModelClass.FindOwnMember(instance.IndexedBy).MemberInfo
        End Function
        Public Shared Function Get_ListView(ByVal instance As IModelListOrderIndex) As IModelListView
            If Not instance.Valid Then Return Nothing
            'Master_Details_ListView
            Dim id As String = instance.ModelClass.TypeInfo.Name & "_" & instance.Member & "_ListView"
            Return DirectCast(instance.Application.Views.GetNode(id), IModelListView)

        End Function
        Public Shared Function Get_MasterMember(ByVal instance As IModelListOrderIndex) As String
            'If Not instance.Valid Then Return Nothing
            Return instance.MasterMemberDataSource.FirstOrDefault
        End Function


        Public Shared Function Get_MasterMemberDataSource(ByVal instance As IModelListOrderIndex) As IEnumerable(Of String)
            If instance.ModelClass Is Nothing Then Return New List(Of String)
            If instance.DetailModelClass Is Nothing Then Return New List(Of String)
            Dim masterTypeName As String = instance.ModelClass.TypeInfo.Name
            'Dim pt As Type = GetType(Persistentbase)
            'Dim parent = DirectCast(instance.Parent, IModelListOrderIndexes)
            'If parent.LongCount(Function(q) q.Id <> instance.Id AndAlso instance.ObjectName.Equals(q.ObjectName)) > 0 Then
            '    Return New List(Of String)
            'End If


            Dim qry = From q In instance.DetailModelClass.OwnMembers
                      Where q.MemberInfo.MemberTypeInfo.IsPersistent AndAlso q.MemberInfo.MemberTypeInfo.Implements(Of IXPSimpleObject) AndAlso q.MemberInfo.MemberTypeInfo.Name = masterTypeName
                      Select q.Name

            'Dim old = From q In parent
            '          Where q.ModelClass.Name = instance.ModelClass.Name AndAlso q.Id <> instance.Id
            '          Select q.Member
            'If instance.DetailModelClass IsNot Nothing Then

            '    Dim old2 = From q In DirectCast(instance.Parent, IModelListOrderIndexes)
            '               Where q.DetailModelClass.Name = instance.DetailModelClass.Name AndAlso q.Id <> instance.Id
            '               Select q.Member

            '    Return qry.Except(old).Except(old2)
            'End If
            'Return qry.Except(old)
            Return qry
        End Function

        Public Shared Function Get_MemberDataSource(ByVal instance As IModelListOrderIndex) As IEnumerable(Of String)
            If instance.ModelClass Is Nothing Then Return New List(Of String)
            Dim parent = DirectCast(instance.Parent, IModelListOrderIndexes)
            'If parent.LongCount(Function(q) q.Id <> instance.Id AndAlso instance.ObjectName.Equals(q.ObjectName)) > 0 Then
            '    Return New List(Of String)
            'End If


            Dim qry = From q In instance.ModelClass.OwnMembers
                      Where q.MemberInfo.IsList AndAlso q.MemberInfo.ListElementTypeInfo.IsPersistent
                      Select q.Name

            'Dim old = From q In parent
            '          Where q.ModelClass.Name = instance.ModelClass.Name AndAlso q.Id <> instance.Id
            '          Select q.Member
            'If instance.DetailModelClass IsNot Nothing Then

            '    Dim old2 = From q In DirectCast(instance.Parent, IModelListOrderIndexes)
            '               Where q.DetailModelClass.Name = instance.DetailModelClass.Name AndAlso q.Id <> instance.Id
            '               Select q.Member

            '    Return qry.Except(old).Except(old2)
            'End If
            'Return qry.Except(old)
            Return qry
        End Function

        Public Shared Function Get_MemberDetailView(ByVal instance As IModelListOrderIndex) As IModelDetailView
            If Not instance.Valid Then Return Nothing
            'Master_Details_ListView
            Dim id As String = instance.DetailModelClass.TypeInfo.Name & "_DetailView"
            Return DirectCast(instance.Application.Views.GetNode(id), IModelDetailView)

        End Function

        Public Shared Function Get_Valid(ByVal instance As IModelListOrderIndex) As Boolean
            If String.IsNullOrEmpty(instance.IndexedBy) Then Return False
            If String.IsNullOrEmpty(instance.Member) Then Return False
            If String.IsNullOrEmpty(instance.ObjectName) Then Return False

            Dim parent = DirectCast(instance.Parent, IModelListOrderIndexes)
            If parent.LongCount(Function(q) q.Id <> instance.Id AndAlso instance.ObjectName.Equals(q.ObjectName) AndAlso instance.Member.Equals(q.Member)) > 0 Then
                Return False
            End If
            If instance.DetailModelClass Is Nothing Then Return False
            If Not instance.DetailModelClass.OwnMembers.Any(Function(q) q.Name = instance.IndexedBy AndAlso q.MemberInfo.MemberType.IsNumericType()) Then Return False
            Return True
        End Function
        Public Shared Function Get_ValidActiveCriteriaOperator(ByVal instance As IModelListOrderIndex) As XNullable(Of CriteriaOperator)
            If Not instance.Valid Then Return XNullable(Of CriteriaOperator).Empty
            If String.IsNullOrEmpty(instance.ActiveCriteria) OrElse ModuleCore.XApplication Is Nothing Then Return XNullable(Of CriteriaOperator).Empty
            Dim ci As XPClassInfo = instance.DetailModelClass.TypeInfo.GetClassInfo()
            Dim iObjectSpace As IObjectSpace = ModuleCore.XApplication.CreateObjectSpace()
            If iObjectSpace Is Nothing OrElse TypeOf iObjectSpace IsNot Xpo.XPObjectSpace Then Return XNullable(Of CriteriaOperator).Empty
            Dim result As CriteriaOperator = Nothing
            Try
                result = DevExpress.Xpo.Helpers.PersistentCriterionExpander.Expand(ci, DirectCast(iObjectSpace, Xpo.XPObjectSpace).Session, CriteriaOperator.TryParse(instance.ActiveCriteria)).ExpandedCriteria
            Catch ex As Exception

            End Try

            If result Is Nothing OrElse Not result.Equals(CriteriaOperator.TryParse(instance.ActiveCriteria)) Then Return XNullable(Of CriteriaOperator).Empty
            Return New XNullable(Of CriteriaOperator)(result)
        End Function

    End Class
End Namespace

