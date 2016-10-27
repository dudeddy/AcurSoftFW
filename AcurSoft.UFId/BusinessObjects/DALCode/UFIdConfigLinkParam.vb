Imports System
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports System.ComponentModel
Namespace UFId.DAL

    Partial Public Class UFIdConfigLinkParam
        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
        End Sub


        '<PersistentAlias("EvaluateExpression(Link.Expression)")>
        'Public ReadOnly Property ParamValue() As Object
        '    Get
        '        Return EvaluateAlias(NameOf(ParamValue))
        '    End Get
        'End Property
    End Class

End Namespace
