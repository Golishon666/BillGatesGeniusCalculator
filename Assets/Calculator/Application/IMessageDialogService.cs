using Cysharp.Threading.Tasks;

namespace BillGatesGeniusCalculator.Calculator.Application
{
    public interface IMessageDialogService
    {
        UniTask ShowAsync(string message, string buttonText);
    }
}
