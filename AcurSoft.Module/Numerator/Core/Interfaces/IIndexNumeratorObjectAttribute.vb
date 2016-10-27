Namespace Numerator.Core

    Public Interface IIndexNumeratorObjectAttribute
        Inherits IIndexCoreNumerator
        ReadOnly Property IndexedBy As String
        ReadOnly Property Id As String
        ReadOnly Property IsEditable As Boolean
        ReadOnly Property ObjectName As String

        Property AllowIndexEdit As Boolean
        Property EditMask As String
        Property DisplayFormat As String

    End Interface
End Namespace
