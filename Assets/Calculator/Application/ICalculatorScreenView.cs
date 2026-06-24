using System.Collections.Generic;

namespace BillGatesGeniusCalculator.Calculator.Application
{
    public interface ICalculatorScreenView
    {
        void Initialize(ICalculatorScreenPresenter presenter, CalculatorScreenViewConfig config);
        void SetInput(string input);
        void SetHistory(IReadOnlyList<string> history);
        void SetInteractable(bool isInteractable);
    }
}
