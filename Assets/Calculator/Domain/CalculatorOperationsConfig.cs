using UnityEngine;

namespace BillGatesGeniusCalculator.Calculator.Domain
{
    [CreateAssetMenu(
        fileName = "CalculatorOperationsConfig",
        menuName = "Bill Gates Genius Calculator/Operations Config")]
    public sealed class CalculatorOperationsConfig : ScriptableObject, ICalculatorOperations
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
