Imports DevExpress.Persistent.Base
Imports DevExpress.Xpo
<DefaultClassOptions>
Public Class Detail
    Inherits XPObject
    'Implements IIndexedObject

    Public Sub New(ByVal session As Session)
        MyBase.New(session)
    End Sub
    Public Overrides Sub AfterConstruction()
        MyBase.AfterConstruction()
    End Sub

    Dim _Index As Decimal
    Public Property Index() As Decimal 'Implements IIndexedObject.Index
        Get
            Return _Index
        End Get
        Set(ByVal value As Decimal)
            SetPropertyValue(Of Decimal)("Index", _Index, value)
        End Set
    End Property


    Dim fCode As String
    Public Property Code() As String
        Get
            Return fCode
        End Get
        Set(ByVal value As String)
            SetPropertyValue(Of String)("Code", fCode, value)
        End Set
    End Property


    Dim fMaster As Master
    <Association("DetailsReferencesMaster")>
    Public Property Master() As Master
        Get
            Return fMaster
        End Get
        Set(ByVal value As Master)
            SetPropertyValue(Of Master)("Master", fMaster, value)
        End Set
    End Property

    Dim fMasterX As MasterX
    <Association("DetailsReferencesMasterX")>
    Public Property MasterX() As MasterX
        Get
            Return fMasterX
        End Get
        Set(ByVal value As MasterX)
            SetPropertyValue(Of MasterX)("MasterX", fMasterX, value)
        End Set
    End Property
End Class
