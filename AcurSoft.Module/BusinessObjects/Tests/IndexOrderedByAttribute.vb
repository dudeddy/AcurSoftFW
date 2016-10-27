Friend Class IndexOrderedByAttribute
    Inherits Attribute

    Public ReadOnly Property MasterProprietyName As String

    Public Sub New(masterProprietyName As String)
        Me.MasterProprietyName = masterProprietyName
    End Sub

End Class
