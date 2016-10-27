Imports AcurSoft.Numerator.Core

Namespace Numerator.Core

    Public Interface IIndexNumeratorListAttribute
        Inherits IIndexNumeratorObjectAttribute
        ReadOnly Property MasterMember As String
        ReadOnly Property ActiveCriteria As String
        ReadOnly Property Member As String

    End Interface
End Namespace
