Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports AcurSoft.ModuleCore
Imports DevExpress.Data
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata
Imports AcurSoft.Data

Module Ext
    <Extension()>
    Public Function IsNumericType(type As Type) As Boolean
        Select Case System.Type.GetTypeCode(type)
            Case TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64, TypeCode.Int16, TypeCode.Int32, TypeCode.Int64, TypeCode.Decimal, TypeCode.Double, TypeCode.Single
                Return True
            Case TypeCode.Object
                If type.IsGenericType AndAlso type.GetGenericTypeDefinition() Is GetType(Nullable(Of )) Then
                    Return Nullable.GetUnderlyingType(type).IsNumericType()
                End If
        End Select
        Return False

    End Function

    <Extension>
    Public Function OrderBy(Of TSource, TKey)(ByVal source As IQueryable(Of TSource), ByVal keySelector As System.Linq.Expressions.Expression(Of Func(Of TSource, TKey)), ByVal sortOrder As System.ComponentModel.ListSortDirection) As IOrderedQueryable(Of TSource)
        If sortOrder = System.ComponentModel.ListSortDirection.Ascending Then
            Return source.OrderBy(keySelector)
        Else
            Return source.OrderByDescending(keySelector)
        End If
    End Function

    <Extension>
    Public Function OrderBy(Of TSource, TKey)(ByVal source As IQueryable(Of TSource), ByVal keySelector As System.Linq.Expressions.Expression(Of Func(Of TSource, TKey)), ByVal sortOrder As ColumnSortOrder) As IOrderedQueryable(Of TSource)
        Select Case sortOrder
            Case ColumnSortOrder.Ascending
                Return source.OrderBy(keySelector)
            Case ColumnSortOrder.Descending
                Return source.OrderByDescending(keySelector)
        End Select
        Return source
    End Function

    <Extension()>
    Public Function GetClassInfo(ByVal type As Type) As XPClassInfo
        Return XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary.QueryClassInfo(type)
        'Dim ti As ITypeInfo = XafTypesInfo.Instance.FindTypeInfo(type)
        'If ti Is Nothing Then Return Nothing
        'Dim ci As XPClassInfo = (New ReflectionDictionary()).QueryClassInfo(ti.AssemblyInfo.FullName, ti.FullName)
        'Return ti.GetClassInfo()
    End Function

    <Extension()>
    Public Function GetClassInfo(ByVal typeInfo As ITypeInfo) As XPClassInfo
        Dim ci As XPClassInfo = (New ReflectionDictionary()).QueryClassInfo(typeInfo.AssemblyInfo.FullName, typeInfo.FullName)
        Return ci
    End Function


    '<Extension()>
    'Public Function OrderBy(ByVal source As IQueryable, ByVal ordering As String, ByVal ParamArray values() As Object) As IQueryable
    '    If (source Is Nothing) Then Throw New ArgumentNullException("source")
    '    If (ordering Is Nothing) Then Throw New ArgumentNullException("ordering")
    '    Dim parameters = New ParameterExpression() {
    '        Expression.Parameter(source.ElementType, "")}
    '    Dim parser As New ExpressionParser(parameters, ordering, values)
    '    Dim orderings As IEnumerable(Of DynamicOrdering) = parser.ParseOrdering()
    '    Dim queryExpr As Expression = source.Expression
    '    Dim methodAsc = "OrderBy"
    '    Dim methodDesc = "OrderByDescending"
    '    For Each o As DynamicOrdering In orderings
    '        queryExpr = Expression.Call(
    '            GetType(Queryable), If(o.Ascending, methodAsc, methodDesc),
    '            New Type() {source.ElementType, o.Selector.Type},
    '            queryExpr, Expression.Quote(Expression.Lambda(o.Selector, parameters)))
    '        methodAsc = "ThenBy"
    '        methodDesc = "ThenByDescending"
    '    Next o
    '    Return source.Provider.CreateQuery(queryExpr)
    'End Function
    <Extension>
    Public Function OrderBy(Of TEntity)(ByVal source As IQueryable(Of TEntity), ByVal orderByProperty As String, ByVal desc As Boolean) As IQueryable(Of TEntity)
        Dim command As String = If(desc, "OrderByDescending", "OrderBy")
        Dim type = GetType(TEntity)
        Dim [property] = type.GetProperty(orderByProperty)
        Dim parameter = Expression.Parameter(type, "p")
        Dim propertyAccess = Expression.MakeMemberAccess(parameter, [property])
        Dim orderByExpression = Expression.Lambda(propertyAccess, parameter)
        Dim resultExpression = Expression.Call(GetType(Queryable), command, New Type() {type, [property].PropertyType}, source.Expression, Expression.Quote(orderByExpression))
        Return source.Provider.CreateQuery(Of TEntity)(resultExpression)
    End Function
    'Private Function FuncToExpression(Of T)(ByVal f As Func(Of T, Boolean)) As Expression(Of Func(Of T, Boolean))
    '    Return f(x)
    'End Function


    <Extension>
    Public Function OrderBy(Of TEntity As PersistentBase)(ByVal source As IQueryable(Of TEntity), ByVal dic As IEnumerable(Of ColumnSortOrderInfo)) As IQueryable(Of TEntity)
        If source.Count = 0 Then Return source
        Dim type = GetType(TEntity)
        Dim queryExpr As Expression = source.Expression
        Dim methodAsc = "OrderBy"
        Dim methodDesc = "OrderByDescending"
        Dim o As TEntity = source.FirstOrDefault
        Dim ci As Metadata.XPClassInfo = o.ClassInfo
        Dim qry = From kv In dic
                  Select kv, m = kv.IsValid(ci)
                  Where m.HasValue
                  Order By kv.Index
                  Select SortOrder = kv.ColumnSortOrder, Member = m.Value, AccessFunc = Function(q) m.Value.GetValue(q)

        If qry.Count = 0 Then Return source

        For Each kv In qry
            Dim sortOrder As String = If(kv.SortOrder = ColumnSortOrder.Ascending, methodAsc, methodDesc)
            'Dim propertyAccessFunc As Func(Of PersistentBase, Object) = Function(q) q.ClassInfo.GetMember(kv.Value.Item1).GetValue(q)
            Dim param = Expression.Parameter(GetType(PersistentBase), "e")
            Dim orderByExpression = Expression.Lambda(Of Func(Of PersistentBase, Object))(Expression.Call(Expression.Constant(kv.AccessFunc.Target), kv.AccessFunc.Method, param), param)
            queryExpr = Expression.Call(GetType(Queryable), sortOrder, New Type() {type, GetType(Object)}, queryExpr, Expression.Quote(orderByExpression))

            methodAsc = "ThenBy"
            methodDesc = "ThenByDescending"
        Next
        Return source.Provider.CreateQuery(queryExpr)

    End Function

End Module
