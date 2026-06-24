using System;

namespace BillGatesGeniusCalculator.Calculator.Domain
{
    [Serializable]
    public sealed class CalculatorOperationsConfig
    {
        public bool Addition = true;
        public bool Subtraction;
        public bool Multiplication;
        public bool Division;

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
