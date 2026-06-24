using Cysharp.Threading.Tasks;

namespace BillGatesGeniusCalculator.Calculator.Application
{
    public interface ICalculatorPresenter
    {
        UniTask InitializeAsync();
    }
}
