using BillGatesGeniusCalculator.Calculator.Application;
using BillGatesGeniusCalculator.Calculator.Domain;
using BillGatesGeniusCalculator.Calculator.Infrastructure;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace BillGatesGeniusCalculator.Calculator.Unity.Views
{
    public sealed class CalculatorAppView : LifetimeScope
    {
        [SerializeField] private CalculatorAppViewConfig appConfig = new CalculatorAppViewConfig();
        [SerializeField] private CalculatorScreenViewConfig screenConfig = new CalculatorScreenViewConfig();
        [SerializeField] private CalculatorScreenView screenView;
        [SerializeField] private CalculatorHistoryDebugView historyDebugView;

        private readonly MessageDialogEventService _messageDialogEventService = new MessageDialogEventService();

        public event MessageDialogRequestedHandler MessageDialogRequested
        {
            add => _messageDialogEventService.MessageDialogRequested += value;
            remove => _messageDialogEventService.MessageDialogRequested -= value;
        }

        public void Initialize(CalculatorAppViewConfig config)
        {
            appConfig = config;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            screenConfig.Operations ??= ScriptableObject.CreateInstance<CalculatorOperationsConfig>();
            historyDebugView ??= screenView != null
                ? screenView.GetComponent<CalculatorHistoryDebugView>()
                : null;

            builder.RegisterInstance(appConfig);
            builder.RegisterInstance(screenConfig);
            builder.RegisterInstance(screenConfig.Operations).As<ICalculatorOperations>();
            builder.RegisterInstance(screenView).As<ICalculatorScreenView>();
            builder.RegisterInstance(historyDebugView);
            builder.Register<AdditionExpressionEvaluator>(Lifetime.Singleton).As<IExpressionEvaluator>();
            builder.Register(_ => new UnityJsonStateRepository(), Lifetime.Singleton).As<IStateRepository>();
            builder.RegisterInstance(_messageDialogEventService).As<IMessageDialogService>();
            builder.Register<CalculatorPresenter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterEntryPoint<CalculatorStartup>();
        }
    }
}
