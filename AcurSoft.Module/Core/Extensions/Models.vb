Imports System.Runtime.CompilerServices
Imports AcurSoft.Model
Imports DevExpress.ExpressApp.Model

Module Models
    <Extension()>
    Public Function AppEx(ByVal node As IModelNode) As IModelApplicationAEx
        Return DirectCast(node.Application, IModelApplicationAEx)
    End Function

    <Extension()>
    Public Function Self(ByVal node As IModelNode) As IModelNode
        Return node
    End Function
End Module
