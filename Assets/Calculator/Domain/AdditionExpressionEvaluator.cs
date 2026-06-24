using System;

namespace BillGatesGeniusCalculator.Calculator.Domain
{
    public sealed class AdditionExpressionEvaluator : IExpressionEvaluator
    {
        public ExpressionEvaluation Evaluate(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return ExpressionEvaluation.Error();
            }

            for (var i = 0; i < expression.Length; i++)
            {
                var symbol = expression[i];
                if (!char.IsDigit(symbol) && symbol != '+')
                {
                    return ExpressionEvaluation.Error();
                }
            }

            var terms = expression.Split('+');
            if (terms.Length < 2)
            {
                return ExpressionEvaluation.Error();
            }

            long sum = 0;
            foreach (var term in terms)
            {
                if (term.Length == 0 || !long.TryParse(term, out var value))
                {
                    return ExpressionEvaluation.Error();
                }

                try
                {
                    checked
                    {
                        sum += value;
                    }
                }
                catch (OverflowException)
                {
                    return ExpressionEvaluation.Error();
                }
            }

            return ExpressionEvaluation.Success(sum);
        }
    }
}
