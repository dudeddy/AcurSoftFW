Imports DevExpress.Persistent.Base
Imports DevExpress.Xpo
<DefaultClassOptions>
Public Class DetailX
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
    Dim _Indx As Integer
    Public Property Indx() As Integer 'Implements IIndxedObject.Indx
        Get
            Return _Indx
        End Get
        Set(ByVal value As Integer)
            SetPropertyValue(Of Integer)("Indx", _Indx, value)
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
    '<IndexOrderedBy("Index")>
    <Association("DetailXsReferencesMaster")>
    Public Property Master() As Master
        Get
            Return fMaster
        End Get
        Set(ByVal value As Master)
            SetPropertyValue(Of Master)("Master", fMaster, value)
        End Set
    End Property
End Class
