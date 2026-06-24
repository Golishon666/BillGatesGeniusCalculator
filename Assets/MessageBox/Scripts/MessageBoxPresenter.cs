using Cysharp.Threading.Tasks;

namespace BillGatesGeniusCalculator.MessageBox
{
    public sealed class MessageBoxPresenter : IMessageBoxPresenter
    {
        private readonly IMessageBoxView _view;
        private UniTaskCompletionSource _completionSource;

        public MessageBoxPresenter(IMessageBoxView view)
        {
            _view = view;
        }

        public UniTask ShowAsync(string message, string buttonText)
        {
            _completionSource ??= new UniTaskCompletionSource();
            _view.SetMessage(message);
            _view.SetButtonText(buttonText);
            _view.Show();
            return _completionSource.Task;
        }

        public void OnConfirmRequested()
        {
            _view.Hide();
            _completionSource?.TrySetResult();
            _completionSource = null;
        }
    }
}
