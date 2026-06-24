using Cysharp.Threading.Tasks;

namespace BillGatesGeniusCalculator.MessageBox
{
    public interface IMessageDialogService
    {
        UniTask ShowAsync(string message, string buttonText);
    }
}
