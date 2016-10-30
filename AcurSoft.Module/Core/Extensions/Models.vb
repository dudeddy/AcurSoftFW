Imports System.Runtime.CompilerServices
Imports AcurSoft.Model
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model

Module Models
    <Extension()>
    Public Function AppEx(ByVal node As IModelNode) As IModelApplicationAEx
        Return DirectCast(node.Application, IModelApplicationAEx)
    End Function
    <Extension()>
    Public Function ModelAppEx(ByVal viewController As ViewController) As IModelApplicationAEx
        Return DirectCast(viewController?.View?.Model?.Application, IModelApplicationAEx)
    End Function

    <Extension()>
    Public Function Self(ByVal node As IModelNode) As IModelNode
        Return node
    End Function
    <Extension()>
    Public Function GetClassEx(ByVal node As IModelNode, type As Type) As IModelClassAEx
        Return DirectCast(node.Application.BOModel.GetClass(type), IModelClassAEx)
    End Function
    <Extension()>
    Public Function GetModelClassEx(ByVal viewController As ViewController) As IModelClassAEx
        Return DirectCast(viewController?.View?.Model?.Application.BOModel.GetClass(viewController?.View?.ObjectTypeInfo.Type), IModelClassAEx)
    End Function

    <Extension()>
    Public Function IModelMemberAEx(ByVal node As IModelMember) As IModelMemberAEx
        Return DirectCast(node, IModelMemberAEx)
    End Function

    <Extension()>
    Public Function IModelMemberAEx(ByVal memberInfo As IMemberInfo, Optional modelApplication As IModelApplication = Nothing) As IModelMemberAEx
        If modelApplication Is Nothing Then
            modelApplication = ModuleCore.XApplication.Model
        End If
        Return modelApplication.BOModel.GetClass(memberInfo.Owner.Type).FindMember(memberInfo.Name)?.IModelMemberAEx()
    End Function
End Module
