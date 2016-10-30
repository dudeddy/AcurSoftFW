Imports System
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.Persistent.Base
Imports AcurSoft.Numerator.Core

Namespace DAL
    <DefaultClassOptions>
    <ListNumeratorAttribute("ObjectTemplateMembers", "Index", "Template")>
    Partial Public Class ObjectTemplate
        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
        End Sub
    End Class

End Namespace
