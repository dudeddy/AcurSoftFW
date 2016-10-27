Imports System
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.Persistent.Base

Namespace UFId.DAL
    <DefaultClassOptions>
    Partial Public Class UFIdConfig
        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
            Me.SequenceStart = 1
            Me.SequenceStep = 1
            Me.ReUseStrategy = Enums.ReUseStrategies.None
        End Sub
    End Class

End Namespace
