using VContainer.Unity;

namespace BillGatesGeniusCalculator.MessageBox
{
    public sealed class MessageBoxStartup : IStartable
    {
        private readonly IMessageBoxView _view;
        private readonly IMessageBoxPresenter _presenter;
        private readonly MessageBoxViewConfig _config;

        public MessageBoxStartup(
            IMessageBoxView view,
            IMessageBoxPresenter presenter,
            MessageBoxViewConfig config)
        {
            _view = view;
            _presenter = presenter;
            _config = config;
        }

        public void Start()
        {
            _view.Initialize(_presenter, _config);
        }
    }
}
