Imports AcurSoft.Numerator.Model
Imports DevExpress.ExpressApp.Model

Namespace Model

    Public Interface IModelApplicationAEx
        Inherits IModelApplication

        ReadOnly Property OrderIndexes As IModelOrderIndexes

    End Interface
End Namespace
