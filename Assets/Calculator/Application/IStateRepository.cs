using BillGatesGeniusCalculator.Calculator.Domain;
using Cysharp.Threading.Tasks;

namespace BillGatesGeniusCalculator.Calculator.Application
{
    public interface IStateRepository
    {
        UniTask<CalculatorState> LoadAsync();
        UniTask SaveAsync(CalculatorState state);
    }
}
