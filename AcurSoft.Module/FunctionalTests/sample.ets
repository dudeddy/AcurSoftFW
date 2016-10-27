;#DropDB XafApplication1EasyTest

#Application XafApplication1.Win
#Application XafApplication1.Web
#IfDef XafApplication1.Win

*Action About...
 
*Action OK

#EndIf

#IfDef XafApplication1.Web
!Action Navigation(My Details)
#EndIf
