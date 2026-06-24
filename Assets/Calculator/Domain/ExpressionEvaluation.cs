namespace BillGatesGeniusCalculator.Calculator.Domain
{
    public readonly struct ExpressionEvaluation
    {
        public ExpressionEvaluation(bool isSuccess, long value)
        {
            IsSuccess = isSuccess;
            Value = value;
        }

        public bool IsSuccess { get; }
        public long Value { get; }

        public static ExpressionEvaluation Success(long value)
        {
            return new ExpressionEvaluation(true, value);
        }

        public static ExpressionEvaluation Error()
        {
            return new ExpressionEvaluation(false, 0);
        }
    }
}
