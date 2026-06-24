using System;
using System.Collections.Generic;

namespace BillGatesGeniusCalculator.Calculator.Domain
{
    public sealed class AdditionExpressionEvaluator : IExpressionEvaluator
    {
        private readonly CalculatorOperationsConfig _operationsConfig;

        public AdditionExpressionEvaluator()
            : this(new CalculatorOperationsConfig())
        {
        }

        public AdditionExpressionEvaluator(CalculatorOperationsConfig operationsConfig)
        {
            _operationsConfig = operationsConfig ?? new CalculatorOperationsConfig();
        }

        public ExpressionEvaluation Evaluate(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return ExpressionEvaluation.Error();
            }

            var values = new List<long>();
            var operations = new List<char>();
            var expectNumber = true;

            for (var index = 0; index < expression.Length; index++)
            {
                var symbol = expression[index];
                if (char.IsDigit(symbol))
                {
                    var value = 0L;
                    while (index < expression.Length && char.IsDigit(expression[index]))
                    {
                        try
                        {
                            checked
                            {
                                value = value * 10 + (expression[index] - '0');
                            }
                        }
                        catch (OverflowException)
                        {
                            return ExpressionEvaluation.Error();
                        }

                        index++;
                    }

                    values.Add(value);
                    index--;
                    expectNumber = false;
                    continue;
                }

                if (!CalculatorOperationsConfig.IsOperationCharacter(symbol) || !_operationsConfig.IsEnabled(symbol))
                {
                    return ExpressionEvaluation.Error();
                }

                if (expectNumber)
                {
                    return ExpressionEvaluation.Error();
                }

                operations.Add(symbol);
                expectNumber = true;
            }

            if (expectNumber || values.Count < 2 || operations.Count != values.Count - 1)
            {
                return ExpressionEvaluation.Error();
            }

            try
            {
                return ExpressionEvaluation.Success(EvaluateWithPrecedence(values, operations));
            }
            catch (OverflowException)
            {
                return ExpressionEvaluation.Error();
            }
            catch (DivideByZeroException)
            {
                return ExpressionEvaluation.Error();
            }
        }

        private static long EvaluateWithPrecedence(IReadOnlyList<long> values, IReadOnlyList<char> operations)
        {
            var collapsedValues = new List<long>();
            var collapsedOperations = new List<char>();
            var current = values[0];

            for (var index = 0; index < operations.Count; index++)
            {
                var operation = operations[index];
                var nextValue = values[index + 1];

                if (operation == '*' || operation == '/')
                {
                    current = ApplyOperation(current, nextValue, operation);
                    continue;
                }

                collapsedValues.Add(current);
                collapsedOperations.Add(operation);
                current = nextValue;
            }

            collapsedValues.Add(current);

            var result = collapsedValues[0];
            for (var index = 0; index < collapsedOperations.Count; index++)
            {
                result = ApplyOperation(result, collapsedValues[index + 1], collapsedOperations[index]);
            }

            return result;
        }

        private static long ApplyOperation(long left, long right, char operation)
        {
            checked
            {
                switch (operation)
                {
                    case '+':
                        return left + right;
                    case '-':
                        return left - right;
                    case '*':
                        return left * right;
                    case '/':
                        return left / right;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}

