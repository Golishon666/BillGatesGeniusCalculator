namespace BillGatesGeniusCalculator.Calculator.Domain
{
    public interface IExpressionEvaluator
    {
        ExpressionEvaluation Evaluate(string expression);
    }
}
