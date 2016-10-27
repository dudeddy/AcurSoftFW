Imports AcurSoft.Numerator.Core
Imports AcurSoft.Numerator.Model
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata

Namespace Numerator

    Public Class IndexListNumerator
        Inherits IndexBaseNumerator



        Public Overrides ReadOnly Property XpIndexMember As IMemberInfo
        Public Overrides ReadOnly Property IndexType As Type
        Public Overrides ReadOnly Property ObjectSpace As IObjectSpace


        Private _CriteriaDown As String
        Public Overrides ReadOnly Property CriteriaDown As String
            Get
                Return _CriteriaDown
            End Get
        End Property

        Public Overrides ReadOnly Property Data As IEnumerable(Of PersistentBase)
            Get
                If Not Me.IsActive Then Return Nothing
                'Return DirectCast(Me.ObjectSpace.Evaluate(Me.TypeInfo.Type, CriteriaOperator.Parse(String.Format("[<{0}>]", Me.TypeInfo.Name)), CriteriaOperator.Parse("1 = 1")), IList).OfType(Of PersistentBase)
                'Return Me.ObjectSpace.CreateCollection(Me.TypeInfo.Type).OfType(Of PersistentBase)
                'Return Me.ListView.CollectionSource.List.OfType(Of PersistentBase)
                If Me.ListView Is Nothing Then
                    If Me.DetailView IsNot Nothing Then


                        Dim master As Object = Me.MasterMember.GetValue(Me.DetailView.CurrentObject)
                        Return DirectCast(Me.ListMember.GetValue(master), IList).OfType(Of PersistentBase).Where(Function(q) Not Me.ObjectSpace.IsNewObject(q))

                    End If

                    'Me.ObjectSpace.CreateCollection(Me.ModelEx.DetailModelClass.TypeInfo.Type, CriteriaOperator.Parse(Me.ModelEx.MasterMember & " = ?", )
                    'Return Me.ObjectSpace.CreateCollection(Me.TypeInfo.Type).OfType(Of PersistentBase)
                Else
                    Return Me.ListView.CollectionSource.List.OfType(Of PersistentBase)
                End If
                Return Nothing

            End Get
        End Property

        Public ReadOnly Property ModelEx As IModelListOrderIndex
        Public ReadOnly Property ListView As ListView
        Public ReadOnly Property DetailView As DetailView

        Public ReadOnly Property MasterMember As IMemberInfo
        Public ReadOnly Property ListMember As IMemberInfo

        Public Overrides Sub SetXpMemberInfo()
            If Me.ModelEx?.DetailModelClass Is Nothing Then Return
            _XpIndexMember = Me.ModelEx.DetailModelClass.TypeInfo.FindMember(Me.IndexMemberName)
            If _XpIndexMember IsNot Nothing Then
                _IndexType = Me.IndexMember.MemberType
            End If
        End Sub

        Private Sub New(model As IModelListOrderIndex)
            MyBase.New(model)
            Me.ModelEx = model
            If Me.IsActive Then
                '_XpIndexMember = Me.ModelEx.DetailModelClass.TypeInfo.FindMember(Me.IndexMemberName)
                Me.SetXpMemberInfo()
                _CriteriaDown = String.Format("{0} < {1}.{2}.Max({0})", Me.IndexMemberName, Me.ModelEx.MasterMember, Me.ModelEx.Member)

            End If
        End Sub
        Public Sub New(model As IModelListOrderIndex, listView As ListView)
            Me.New(model)
            Me.ObjectSpace = listView.ObjectSpace
            'Me.ModelEx = model
            Me.ListView = listView
        End Sub

        'Public Sub New(model As IModelListOrderIndex, objectSpace As IObjectSpace)
        '    MyBase.New(model, objectSpace)
        '    Me.ModelEx = DirectCast(model, IModelListOrderIndex)
        '    If Me.IsActive Then
        '        '_XpIndexMember = Me.ModelEx.DetailModelClass.TypeInfo.GetClassInfo().GetMember(Me.IndexMemberName)

        '        _CriteriaDown = String.Format("{0} < {1}.{2}.Max({0})", Me.IndexMemberName, Me.ModelEx.MasterMember, Me.ModelEx.Member)

        '    End If
        'End Sub
        Public Sub New(model As IModelListOrderIndex, detailView As DetailView)
            MyBase.New(model, detailView.ObjectSpace)
            Me.ModelEx = DirectCast(model, IModelListOrderIndex)
            Me.ObjectSpace = detailView.ObjectSpace
            Me.DetailView = detailView
            If Me.IsActive Then
                '_XpIndexMember = Me.ModelEx.DetailModelClass.TypeInfo.GetClassInfo().GetMember(Me.IndexMemberName)
                Me.SetXpMemberInfo()

                _CriteriaDown = String.Format("{0} < {1}.{2}.Max({0})", Me.IndexMemberName, Me.ModelEx.MasterMember, Me.ModelEx.Member)
                _MasterMember = Me.ModelEx.DetailModelClass.TypeInfo.FindMember(Me.ModelEx.MasterMember)
                _ListMember = Me.ModelEx.ModelClass.TypeInfo.FindMember(Me.ModelEx.Member)
            End If
            '_DataGetter = Function()
            '                  Return listView.GetValue(Of CollectionSourceBase)("CollectionSource").List.OfType(Of PersistentBase)
            '              End Function
            'Me.ObjectSpace = ObjectSpace
            'Me.Model = model
            'Me.IsActive = model.Valid
            'If Me.IsActive Then

            '    Me.TypeInfo = model.ModelClass.TypeInfo
            '    Me.IndexMemberName = model.IndexedBy
            '    Me.IndexMember = model.IndexMember
            '    Me.XpIndexMember = Me.TypeInfo.GetClassInfo().GetMember(Me.IndexMemberName)
            '    Me.IndexType = Me.IndexMember.MemberType
            'End If
        End Sub

    End Class
End Namespace
