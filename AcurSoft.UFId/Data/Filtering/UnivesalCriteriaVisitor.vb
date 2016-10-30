
'Imports AcurSoft.Data.Filtering.Helpers
Imports DevExpress.Data.Filtering

Namespace Data.Filtering


    Public Class UnivesalCriteriaVisitor
        Implements ICriteriaVisitor(Of CriteriaOperator), IClientCriteriaVisitor(Of CriteriaOperator)

        Public Property Tag As Object

        Private _Criteria As CriteriaOperator
        Public ReadOnly Property Criteria As CriteriaOperator
            Get
                Return _Criteria
            End Get
        End Property



        Public Sub New(exp As String)
            _Criteria = CriteriaOperator.Parse(exp) '.Accept(Me)
        End Sub
        Public Sub New(cp As CriteriaOperator)
            _Criteria = cp '.Accept(Me)
        End Sub

        Public Function Process() As CriteriaOperator
            Return _Criteria.Accept(Me)
        End Function

        Public Shared Function Process(cp As CriteriaOperator) As CriteriaOperator
            Return New UnivesalCriteriaVisitor(cp).Process()
        End Function
        'Public Shared Function GetCritriaKind(co As CriteriaOperator) As CriteriaKind
        '    If TypeOf co Is AggregateOperand Then
        '        Return CriteriaKind.AggregateOperand
        '    ElseIf TypeOf co Is BetweenOperator Then
        '        Return CriteriaKind.BetweenOperator
        '    ElseIf TypeOf co Is BinaryOperator Then
        '        Return CriteriaKind.BinaryOperator
        '    ElseIf TypeOf co Is FunctionOperator Then
        '        Return CriteriaKind.FunctionOperator
        '    ElseIf TypeOf co Is GroupOperator Then
        '        Return CriteriaKind.GroupOperator
        '    ElseIf TypeOf co Is InOperator Then
        '        Return CriteriaKind.InOperator
        '    ElseIf TypeOf co Is JoinOperand Then
        '        Return CriteriaKind.JoinOperand
        '    ElseIf TypeOf co Is OperandProperty Then
        '        Return CriteriaKind.OperandProperty
        '    ElseIf TypeOf co Is UnaryOperator Then
        '        Return CriteriaKind.UnaryOperator
        '    ElseIf TypeOf co Is OperandValue Then
        '        Return CriteriaKind.OperandValue
        '    Else
        '        Return CriteriaKind.Unknow
        '    End If

        'End Function

        Public Event OnVisiting(e As OnVisitingArg)
        Public Class OnVisitingArg '(Of T As CriteriaOperator)
            'Public ReadOnly Property Kind As CriteriaKind
            '    Get
            '        Return GetCritriaKind(Me.OriginalCriteria)
            '    End Get
            'End Property


            Public Property OriginalCriteria As CriteriaOperator
            Public Property Handled As Boolean
            Public Property NewCriteria As CriteriaOperator
            'Public Property BeforeNew As Func(Of CriteriaOperator, CriteriaOperator)
            Public Property AfterNew As Action(Of CriteriaOperator, CriteriaOperator)

            Public Property CreateNew As Func(Of CriteriaOperator, CriteriaOperator)

            Public Function Execute() As CriteriaOperator
                If Not Me.Handled Then
                    'If Me.BeforeNew IsNot Nothing Then
                    '    Me.BeforeNew.Invoke(Me.OriginalCriteria, Me.NewCriteria)
                    'End If

                    If Me.CreateNew Is Nothing Then
                        Me.CreateNew = Function(q) q
                    End If
                    Me.NewCriteria = Me.CreateNew.Invoke(Me.OriginalCriteria)

                    If Me.AfterNew IsNot Nothing Then
                        Me.AfterNew.Invoke(Me.OriginalCriteria, Me.NewCriteria)
                    End If
                End If
                Return Me.NewCriteria
            End Function
        End Class

        'Public Class UnivesalVisitor(Of T As CriteriaOperator)
        '    Public ReadOnly Property OriginalCriteria As CriteriaOperator
        '    Public ReadOnly Property CreateNew As Func(Of T, CriteriaOperator)
        '    Public ReadOnly Property VisitingArg As OnVisitingArg

        '    Public Sub New(originalCriteria As CriteriaOperator, createNew As Func(Of T, CriteriaOperator))
        '        _OriginalCriteria = originalCriteria
        '        _CreateNew = createNew
        '        VisitingArg = New OnVisitingArg With {
        '        .OriginalCriteria = originalCriteria,
        '        .CreateNew = DirectCast(createNew, Func(Of CriteriaOperator, CriteriaOperator))
        '        }

        '    End Sub


        'End Class


        Public Overridable Function Visit(op As BetweenOperator) As CriteriaOperator Implements ICriteriaVisitor(Of CriteriaOperator).Visit

            Dim fnc As Func(Of CriteriaOperator, CriteriaOperator) =
            Function(q)
                Dim org As BetweenOperator = DirectCast(q, BetweenOperator)
                Return New BetweenOperator With {
                        .BeginExpression = org.BeginExpression.Accept(Me),
                        .EndExpression = org.EndExpression.Accept(Me),
                        .TestExpression = org.TestExpression.Accept(Me)
                    }
            End Function

            Dim arg As New OnVisitingArg With {
            .OriginalCriteria = op,
            .CreateNew = fnc
        }

            RaiseEvent OnVisiting(arg)
            Return arg.Execute

        End Function

        Public Overridable Function Visit(op As BinaryOperator) As CriteriaOperator Implements ICriteriaVisitor(Of CriteriaOperator).Visit
            Dim fnc As Func(Of CriteriaOperator, CriteriaOperator) =
            Function(q)
                Dim org As BinaryOperator = DirectCast(q, BinaryOperator)
                Return New BinaryOperator With {
                .LeftOperand = org.LeftOperand.Accept(Me),
                .RightOperand = org.RightOperand.Accept(Me),
                .OperatorType = org.OperatorType
            }
            End Function

            Dim arg As New OnVisitingArg With {
            .OriginalCriteria = op,
            .CreateNew = fnc
        }

            RaiseEvent OnVisiting(arg)
            Return arg.Execute
        End Function

        Public Overridable Function Visit(op As FunctionOperator) As CriteriaOperator Implements ICriteriaVisitor(Of CriteriaOperator).Visit
            Dim fnc As Func(Of CriteriaOperator, CriteriaOperator) =
            Function(q)
                Dim org As FunctionOperator = DirectCast(q, FunctionOperator)

                Dim nop As New FunctionOperator
                nop.OperatorType = org.OperatorType
                nop.Operands.AddRange(org.Operands.Select(Function(o) o.Accept(Me)))
                Return nop
            End Function

            Dim arg As New OnVisitingArg With {
            .OriginalCriteria = op,
            .CreateNew = fnc
        }

            RaiseEvent OnVisiting(arg)
            Return arg.Execute
        End Function

        Public Overridable Function Visit(op As GroupOperator) As CriteriaOperator Implements ICriteriaVisitor(Of CriteriaOperator).Visit
            Dim fnc As Func(Of CriteriaOperator, CriteriaOperator) =
            Function(q)
                Dim org As GroupOperator = DirectCast(q, GroupOperator)
                Dim nop As New GroupOperator With {.OperatorType = org.OperatorType}
                nop.Operands.AddRange(org.Operands.Select(Function(o) o.Accept(Me)))
                Return nop
                Return nop
            End Function

            Dim arg As New OnVisitingArg With {
            .OriginalCriteria = op,
            .CreateNew = fnc
        }

            RaiseEvent OnVisiting(arg)
            Return arg.Execute
        End Function

        Public Overridable Function Visit(op As InOperator) As CriteriaOperator Implements ICriteriaVisitor(Of CriteriaOperator).Visit
            Dim fnc As Func(Of CriteriaOperator, CriteriaOperator) =
            Function(q)
                Dim org As InOperator = DirectCast(q, InOperator)

                Dim nop As New InOperator With {.LeftOperand = org.LeftOperand.Accept(Me)}
                nop.Operands.AddRange(org.Operands.Select(Function(o) o.Accept(Me)))
                Return nop
            End Function

            Dim arg As New OnVisitingArg With {
            .OriginalCriteria = op,
            .CreateNew = fnc
        }

            RaiseEvent OnVisiting(arg)
            Return arg.Execute
        End Function

        Public Overridable Function Visit(op As UnaryOperator) As CriteriaOperator Implements ICriteriaVisitor(Of CriteriaOperator).Visit
            Dim fnc As Func(Of CriteriaOperator, CriteriaOperator) =
            Function(q)
                Dim org As UnaryOperator = DirectCast(q, UnaryOperator)

                Return New UnaryOperator With {.Operand = org.Operand.Accept(Me), .OperatorType = org.OperatorType}
            End Function

            Dim arg As New OnVisitingArg With {
            .OriginalCriteria = op,
            .CreateNew = fnc
        }

            RaiseEvent OnVisiting(arg)
            Return arg.Execute
        End Function

        Public Overridable Function Visit(op As AggregateOperand) As CriteriaOperator Implements IClientCriteriaVisitor(Of CriteriaOperator).Visit
            Dim fnc As Func(Of CriteriaOperator, CriteriaOperator) =
            Function(q)
                Dim org As AggregateOperand = DirectCast(q, AggregateOperand)

                Dim ao As New AggregateOperand With {.AggregateType = org.AggregateType}
                If org.CollectionProperty IsNot Nothing Then
                    ao.CollectionProperty = DirectCast(org.CollectionProperty.Accept(Me), OperandProperty)
                End If
                If org.AggregatedExpression IsNot Nothing Then
                    ao.AggregatedExpression = org.AggregatedExpression.Accept(Me)
                End If
                If org.Condition IsNot Nothing Then
                    ao.Condition = org.Condition.Accept(Me)
                End If
                Return ao
            End Function

            Dim arg As New OnVisitingArg With {
            .OriginalCriteria = op,
            .CreateNew = fnc
        }

            RaiseEvent OnVisiting(arg)
            Return arg.Execute


        End Function

        Public Overridable Function Visit(op As JoinOperand) As CriteriaOperator Implements IClientCriteriaVisitor(Of CriteriaOperator).Visit
            Dim fnc As Func(Of CriteriaOperator, CriteriaOperator) =
            Function(q)
                Dim org As JoinOperand = DirectCast(q, JoinOperand)

                Dim jo As New JoinOperand With {.AggregateType = org.AggregateType}
                If org.AggregatedExpression IsNot Nothing Then
                    jo.AggregatedExpression = org.AggregatedExpression.Accept(Me)
                End If
                If org.Condition IsNot Nothing Then
                    jo.Condition = org.Condition.Accept(Me)
                End If
                Return jo
            End Function

            Dim arg As New OnVisitingArg With {
            .OriginalCriteria = op,
            .CreateNew = fnc
        }

            RaiseEvent OnVisiting(arg)
            Return arg.Execute
        End Function

        Public Overridable Function Visit(theOperator As OperandProperty) As CriteriaOperator Implements IClientCriteriaVisitor(Of CriteriaOperator).Visit
            Dim fnc As Func(Of CriteriaOperator, CriteriaOperator) =
            Function(q)
                Dim org As OperandProperty = DirectCast(q, OperandProperty)
                Return New OperandProperty(org.PropertyName)
            End Function

            Dim arg As New OnVisitingArg With {
            .OriginalCriteria = theOperator,
            .CreateNew = fnc
        }

            RaiseEvent OnVisiting(arg)
            Return arg.Execute
        End Function

        Public Overridable Function Visit(theOperator As OperandValue) As CriteriaOperator Implements ICriteriaVisitor(Of CriteriaOperator).Visit
            Dim fnc As Func(Of CriteriaOperator, CriteriaOperator) =
            Function(q)
                Dim org As OperandValue = DirectCast(q, OperandValue)

                Return New OperandValue(org.Value)
            End Function

            Dim arg As New OnVisitingArg With {
            .OriginalCriteria = theOperator,
            .CreateNew = fnc
        }

            RaiseEvent OnVisiting(arg)
            Return arg.Execute
        End Function

    End Class

End Namespace
