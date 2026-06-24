namespace BillGatesGeniusCalculator.Calculator.Domain
{
    public sealed class CalculatorOperations : ICalculatorOperations
    {
        public static readonly CalculatorOperations AdditionOnly = new CalculatorOperations(
            addition: true,
            subtraction: false,
            multiplication: false,
            division: false);

        public CalculatorOperations(
            bool addition,
            bool subtraction,
            bool multiplication,
            bool division)
        {
            Addition = addition;
            Subtraction = subtraction;
            Multiplication = multiplication;
            Division = division;
        }

        public bool Addition { get; }

        public bool Subtraction { get; }

        public bool Multiplication { get; }

        public bool Division { get; }

        public bool IsEnabled(char operation)
        {
            switch (operation)
            {
                case '+':
                    return Addition;
                case '-':
                    return Subtraction;
                case '*':
                    return Multiplication;
                case '/':
                    return Division;
                default:
                    return false;
            }
        }

        public static bool IsOperationCharacter(char symbol)
        {
            return symbol == '+' || symbol == '-' || symbol == '*' || symbol == '/';
        }
    }
}
