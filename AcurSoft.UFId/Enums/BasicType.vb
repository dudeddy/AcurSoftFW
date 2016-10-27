Imports System.Runtime.CompilerServices
Imports AcurSoft.Enums
Imports DevExpress.Data.Filtering
Imports DevExpress.Data.Filtering.Helpers
Imports DevExpress.ExpressApp.Utils.Reflection
Imports DevExpress.Utils.Serializing.Helpers
Imports DevExpress.Xpo
Imports DevExpress.Xpo.Helpers
Imports DevExpress.Xpo.Metadata

Namespace Enums
    Public Enum BasicTypeCode
        [Text] = TypeCode.String
        [Decimal] = TypeCode.Decimal
        [Integer] = TypeCode.Int64
        [DateTime] = TypeCode.DateTime
        [Boolean] = TypeCode.Boolean
    End Enum
End Namespace

Public Class DataHelper
    Public Shared Property CoreXPClassInfo As XPClassInfo = CoreXPDictionary.QueryClassInfo(GetType(PersistentBase))
    Public Shared Property CoreXPDictionary As XPDictionary = New ReflectionDictionary()


End Class

'Public Class BasicTypesHelper
'    Public Shared ReadOnly Property ObjectConverters As ObjectConverterImplementation
'    Public Shared ReadOnly Property Converters As Dictionary(Of BasicTypeCode, IOneTypeObjectConverter)

'    Shared Sub New()
'        ObjectConverters = New ObjectConverterImplementation
'        'ObjectConverters.
'        'Converters = New Dictionary(Of BasicTypeCode, IOneTypeObjectConverter)
'        '_Converters.Add(BasicTypeCode.Decimal, ObjectConverters.GetConverter(GetType(Decimal)))
'        '_Converters.Add(BasicTypeCode.Integer, ObjectConverters.GetConverter(GetType(Int64)))
'        '_Converters.Add(BasicTypeCode.DateTime, ObjectConverters.GetConverter(GetType(DateTime)))
'        '_Converters.Add(BasicTypeCode.Boolean, ObjectConverters.GetConverter(GetType(Boolean)))
'    End Sub
'End Class

Partial Public Module Ext


    <Extension()>
    Public Function ToType(targetType As BasicTypeCode) As Type
        Select Case targetType
            Case BasicTypeCode.Boolean
                Return GetType(Boolean)
            Case BasicTypeCode.Integer
                Return GetType(Int64)
            Case BasicTypeCode.DateTime
                Return GetType(DateTime)
            Case BasicTypeCode.Decimal
                Return GetType(Decimal)
            Case BasicTypeCode.Text
                Return GetType(String)
        End Select
        Return Nothing
    End Function

    <Extension()>
    Public Function ToObject(ByVal s As String, targetType As BasicTypeCode) As Object
        If targetType = BasicTypeCode.Text Then Return s
        If String.IsNullOrEmpty(s) Then Return Nothing
        Return DevExpress.Utils.Serializing.Helpers.ObjectConverter.StringToObject(s, targetType.ToType())
    End Function

    <Extension()>
    Public Function ToBasicTypeCode(ByVal s As String) As BasicTypeCode
        Return DirectCast(New ExpressionEvaluator(DirectCast(Nothing, EvaluatorContextDescriptor), CriteriaOperator.Parse(s)).Evaluate(Nothing), BasicTypeCode)
    End Function

    <Extension()>
    Public Function ObjectToString(sourceType As BasicTypeCode, ByVal o As Object) As String
        Return DevExpress.Utils.Serializing.Helpers.ObjectConverter.ObjectToString(o)
    End Function

    <Extension()>
    Public Function FromString(basicType As BasicTypeCode) As String
        Return Convert.ToString(New ExpressionEvaluator(DirectCast(Nothing, EvaluatorContextDescriptor), CriteriaOperator.Parse("?", basicType)).Evaluate(Nothing))
    End Function

    <Extension()>
    Public Function ToCriteriaOperator(basicType As BasicTypeCode) As CriteriaOperator
        Return CriteriaOperator.Parse("?", basicType)
    End Function

    <Extension()>
    Public Function CoreEvaluate(criteriaOperator As CriteriaOperator) As Object
        If criteriaOperator Is Nothing Then Return Nothing
        Return New ExpressionEvaluator(DirectCast(Nothing, EvaluatorContextDescriptor), criteriaOperator).Evaluate(Nothing)
    End Function

    <Extension()>
    Public Function CoreEvaluate(expression As String) As Object
        If String.IsNullOrEmpty(expression) Then Return Nothing
        Return expression.GetSafeCoreCriteriaOperator()?.CoreEvaluate()
    End Function

    <Extension()>
    Public Function GetSafeCoreCriteriaOperator(expression As String) As CriteriaOperator
        If String.IsNullOrEmpty(expression) Then Return Nothing
        Dim criteriaOperator As CriteriaOperator = CriteriaOperator.TryParse(expression)
        If criteriaOperator Is Nothing Then Return Nothing
        criteriaOperator = PersistentCriterionExpander.Expand(DataHelper.CoreXPClassInfo, DirectCast(Nothing, IPersistentValueExtractor), criteriaOperator).ExpandedCriteria
        Return criteriaOperator
    End Function

End Module