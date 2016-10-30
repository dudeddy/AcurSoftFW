Imports System
Imports AcurSoft.Enums
Imports DevExpress.Data.Filtering
Imports DevExpress.Data.Filtering.Helpers
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB

Namespace Data.Filtering
    Public Class EvaluateExpressionFunction
        Implements ICustomFunctionOperatorBrowsable
        Public Const FunctionName As String = "EvaluateExpression"

        '#Region "ICustomFunctionOperatorFormattable Members"
        '    ' The function's expression to be evaluated on the server. 
        '    Private Function Format(ByVal providerType As Type, ParamArray ByVal operands() As String) As String Implements ICustomFunctionOperatorFormattable.Format
        '        ' This example implements the function for MS Access databases only. 
        '        If providerType IsNot GetType(MSSqlProviderFactory) Then
        '            Throw New NotSupportedException(String.Concat("This provider is not supported: ", providerType.Name))
        '        End If
        '        'Dim t As BasicTypeCode = DirectCast(New ExpressionEvaluator(DirectCast(Nothing, EvaluatorContextDescriptor), CriteriaOperator.Parse(operands(1))).Evaluate(Nothing), BasicTypeCode)
        '        Dim t As BasicTypeCode = DirectCast(CriteriaOperator.Parse(operands(1)).Evaluate(), BasicTypeCode)
        '        Select Case t
        '            Case BasicTypeCode.Boolean
        '                Return String.Format("CONVERT(Bit, '{0}')", operands(0))
        '            Case BasicTypeCode.Decimal
        '                Return String.Format("CAST(ISNULL('{0}',0) AS DECIMAL(12,6))", operands(0))
        '            Case BasicTypeCode.DateTime
        '                Return String.Format("CONVERT(datetime, '{0}', 101)", operands(0))
        '            Case BasicTypeCode.Integer
        '                Return String.Format("CONVERT(INT, '{0}')", operands(0))
        '            Case BasicTypeCode.Text
        '                Return String.Format("'{0}'", operands(0))
        '        End Select
        '        Return String.Format("'{0}'", operands(0))
        '    End Function
        '#End Region

#Region "ICustomFunctionOperator Members"
        ' Evaluates the function on the client. 
        Private Function Evaluate(ParamArray ByVal operands() As Object) As Object Implements ICustomFunctionOperator.Evaluate
            Dim s As String = Convert.ToString(operands(0))
            Select Case operands.Length
                Case 1
                    Return s.CoreEvaluate()
                Case 2
                    If TypeOf operands(1) Is XPBaseObject Then
                        Return DirectCast(operands(1), XPBaseObject).Evaluate(s)
                    End If
            End Select
            Return Nothing
        End Function

        Private ReadOnly Property Name() As String Implements ICustomFunctionOperator.Name
            Get
                Return FunctionName
            End Get
        End Property


        Private Function ResultType(ParamArray ByVal operands() As Type) As Type Implements ICustomFunctionOperator.ResultType
            Return GetType(Object)
        End Function

#End Region

#Region "ICustomFunctionOperatorBrowsable Members"


        Public ReadOnly Property MinOperandCount As Integer Implements ICustomFunctionOperatorBrowsable.MinOperandCount
            Get
                Return 1
            End Get
        End Property

        Public ReadOnly Property MaxOperandCount As Integer Implements ICustomFunctionOperatorBrowsable.MaxOperandCount
            Get
                Return 2
            End Get
        End Property

        Public ReadOnly Property Description As String Implements ICustomFunctionOperatorBrowsable.Description
            Get
                Return "Evaluate Expression"
            End Get
        End Property

        Public ReadOnly Property Category As FunctionCategory Implements ICustomFunctionOperatorBrowsable.Category
            Get
                Return FunctionCategory.Text
            End Get
        End Property

        Public Function IsValidOperandCount(count As Integer) As Boolean Implements ICustomFunctionOperatorBrowsable.IsValidOperandCount
            Return count = 1 OrElse count = 2
        End Function

        Public Function IsValidOperandType(operandIndex As Integer, operandCount As Integer, type As Type) As Boolean Implements ICustomFunctionOperatorBrowsable.IsValidOperandType
            If operandIndex = 0 Then
                Return type Is GetType(String)
            ElseIf operandIndex = 1 AndAlso operandCount = 2
                Return GetType(XPBaseObject).IsAssignableFrom(type)
            End If
            Return False
        End Function
#End Region
    End Class

End Namespace
