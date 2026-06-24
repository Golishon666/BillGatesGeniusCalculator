namespace BillGatesGeniusCalculator.MessageBox
{
    public interface IMessageBoxView
    {
        void Initialize(IMessageBoxPresenter presenter, MessageBoxViewConfig config);
        void Show();
        void Hide();
        void SetMessage(string message);
        void SetButtonText(string buttonText);
    }
}
