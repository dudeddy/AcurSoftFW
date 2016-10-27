Imports DevExpress.Persistent.Base
Imports DevExpress.Xpo
Imports AcurSoft.Helpers

<DefaultClassOptions>
Public Class MasterX
    Inherits XPObject
    Public Sub New(ByVal session As Session)
        MyBase.New(session)
    End Sub
    Public Overrides Sub AfterConstruction()
        MyBase.AfterConstruction()
    End Sub

    Dim fCode As String
    Public Property Code() As String
        Get
            Return fCode
        End Get
        Set(ByVal value As String)
            SetPropertyValue(Of String)("Code", fCode, value)
        End Set
    End Property

    <Association("DetailsReferencesMasterX"), Aggregated()>
    Public ReadOnly Property Details() As XPCollection(Of Detail)
        Get
            Return GetCollection(Of Detail)("Details")
        End Get
    End Property


    Dim _Index As Decimal
    Public Property Index() As Decimal 'Implements IIndexedObject.Index
        Get
            Return _Index
        End Get
        Set(ByVal value As Decimal)
            SetPropertyValue(Of Decimal)("Index", _Index, value)
        End Set
    End Property
End Class
