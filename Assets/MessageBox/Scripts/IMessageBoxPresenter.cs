using Cysharp.Threading.Tasks;

namespace BillGatesGeniusCalculator.MessageBox
{
    public interface IMessageBoxPresenter
    {
        UniTask ShowAsync(string message, string buttonText);
        void OnConfirmRequested();
    }
}
