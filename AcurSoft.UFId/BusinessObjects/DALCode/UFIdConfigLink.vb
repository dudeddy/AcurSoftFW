Imports System
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base
Imports AcurSoft.UFId.Enums
Imports AcurSoft.UFId.Utils

Namespace UFId.DAL
    <DefaultClassOptions>
    Partial Public Class UFIdConfigLink

        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
            Me.DateExpression = "Today()"
            Me.DateTextExpression = String.Format("Year({0})", UFIdSequenceInfos.PARAM_DATE)
            Me.ValueExpression = String.Format("{0} + '_' + {1}", UFIdSequenceInfos.PARAM_DATE_EXP, UFIdSequenceInfos.PARAM_INDEX)
            Me.AppyCondition = "IsNewObject(This)"
        End Sub

        Dim _LastUsedNumValue As Long?
        Public Property LastUsedNumValue() As Long?
            Get
                Return _LastUsedNumValue
            End Get
            Set(ByVal value As Long?)
                SetPropertyValue(Of Long?)("LastUsedNumValue", _LastUsedNumValue, value)
            End Set
        End Property

        Dim _LastUsedDateValue As Date?
        Public Property LastUsedDateValue() As Date?
            Get
                Return _LastUsedDateValue
            End Get
            Set(ByVal value As Date?)
                SetPropertyValue(Of Date?)("LastUsedDateValue", _LastUsedDateValue, value)
            End Set
        End Property

        Protected Overrides Sub OnSaved()
            MyBase.OnSaved()
            UFIdInitializer.Refresh()
        End Sub

        Protected Overrides Sub OnDeleted()
            MyBase.OnDeleted()
            UFIdInitializer.Refresh()
        End Sub

        Public ReadOnly Property DeletedSequences As IEnumerable(Of UFId.DAL.UFIdSequence)
            Get
                Return Me.UFIdSequences.Where(Function(q) q.Status = SequenceStatuses.Deleted).OrderBy(Function(q) q.NumValue)
            End Get
        End Property

        Public ReadOnly Property ReservedSequences As IEnumerable(Of UFId.DAL.UFIdSequence)
            Get
                Return Me.UFIdSequences.Where(Function(q) q.Status = SequenceStatuses.Reserved).OrderBy(Function(q) q.NumValue)
            End Get
        End Property

        Public Function GetTargetTypeInfo() As ITypeInfo
            If String.IsNullOrEmpty(Me.TargetTypeName) Then Return Nothing
            Return XafTypesInfo.Instance.FindTypeInfo(Me.TargetTypeName)
        End Function


        'Public Function GetTargetObject(session As Session, targetId As Guid) As BaseObject
        '    Dim ti As ITypeInfo = Me.GetTargetTypeInfo()
        '    If ti Is Nothing Then Return Nothing

        '    Return DirectCast(session.GetObjectByKey(ti.Type, targetId), BaseObject)
        'End Function


    End Class

End Namespace
