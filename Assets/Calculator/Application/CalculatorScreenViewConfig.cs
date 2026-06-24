using System;

namespace BillGatesGeniusCalculator.Calculator.Application
{
    [Serializable]
    public sealed class CalculatorScreenViewConfig
    {
        public string Title = "CALCULATOR PRO \u00AE";
        public string Placeholder = "Enter an equation...";
        public string ResultButtonText = "RESULT";
        public string ErrorDialogMessage = "Please check the expression\nyou just entered";
        public string ErrorDialogButtonText = "GOT IT";
    }
}
