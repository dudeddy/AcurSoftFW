Namespace Core

    Public Class XNullable(Of T)

        Public Shared ReadOnly Property Empty As XNullable(Of T)
            Get
                Return New XNullable(Of T)(Nothing)
            End Get
        End Property

        Public ReadOnly Property Value As T
        Public ReadOnly Property HasValue As Boolean

        Public Sub New(o As T)
            Me.Value = o
            Me.HasValue = o IsNot Nothing
        End Sub

    End Class
End Namespace
