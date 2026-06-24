using BillGatesGeniusCalculator.Calculator.Application;
using BillGatesGeniusCalculator.Calculator.Unity.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace BillGatesGeniusCalculator.Calculator.Unity
{
    public sealed class CalculatorStartup : IStartable
    {
        private readonly ICalculatorPresenter _calculatorPresenter;
        private readonly ICalculatorScreenView _screenView;
        private readonly CalculatorScreenViewConfig _screenConfig;
        private readonly CalculatorHistoryDebugView _historyDebugView;

        public CalculatorStartup(
            ICalculatorPresenter calculatorPresenter,
            ICalculatorScreenView screenView,
            CalculatorScreenViewConfig screenConfig,
            CalculatorHistoryDebugView historyDebugView)
        {
            _calculatorPresenter = calculatorPresenter;
            _screenView = screenView;
            _screenConfig = screenConfig;
            _historyDebugView = historyDebugView;
        }

        public void Start()
        {
            _screenView.Initialize(_calculatorPresenter, _screenConfig);
            _historyDebugView?.Initialize(_calculatorPresenter);
            _calculatorPresenter.InitializeAsync().Forget(LogStartupFailure);
        }

        private static void LogStartupFailure(System.Exception exception)
        {
            if (exception != null)
            {
                Debug.LogError($"Calculator initialization failed: {exception}");
            }
        }
    }
}
