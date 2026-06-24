namespace BillGatesGeniusCalculator.Calculator.Application
{
    public interface ICalculatorScreenPresenter
    {
        void OnInputChanged(string input);
        void OnResultRequested(string input);
        void ClearHistoryDebug();
    }
}
