using BillGatesGeniusCalculator.Calculator.Application;
using BillGatesGeniusCalculator.MessageBox;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace BillGatesGeniusCalculator.Calculator.Unity
{
    public sealed class CalculatorStartup : IStartable
    {
        private readonly ICalculatorPresenter _calculatorPresenter;
        private readonly ICalculatorScreenPresenter _screenPresenter;
        private readonly ICalculatorScreenView _screenView;
        private readonly CalculatorScreenViewConfig _screenConfig;
        private readonly IMessageBoxPresenter _messageBoxPresenter;
        private readonly IMessageBoxView _messageBoxView;
        private readonly MessageBoxViewConfig _messageBoxConfig;

        public CalculatorStartup(
            ICalculatorPresenter calculatorPresenter,
            ICalculatorScreenPresenter screenPresenter,
            ICalculatorScreenView screenView,
            CalculatorScreenViewConfig screenConfig,
            IMessageBoxPresenter messageBoxPresenter,
            IMessageBoxView messageBoxView,
            MessageBoxViewConfig messageBoxConfig)
        {
            _calculatorPresenter = calculatorPresenter;
            _screenPresenter = screenPresenter;
            _screenView = screenView;
            _screenConfig = screenConfig;
            _messageBoxPresenter = messageBoxPresenter;
            _messageBoxView = messageBoxView;
            _messageBoxConfig = messageBoxConfig;
        }

        public void Start()
        {
            _messageBoxView.Initialize(_messageBoxPresenter, _messageBoxConfig);
            _screenView.Initialize(_screenPresenter, _screenConfig);
            _calculatorPresenter.InitializeAsync().Forget();
        }
    }
}
