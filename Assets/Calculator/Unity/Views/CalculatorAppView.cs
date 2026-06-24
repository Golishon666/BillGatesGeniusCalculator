using BillGatesGeniusCalculator.Calculator.Application;
using BillGatesGeniusCalculator.Calculator.Domain;
using BillGatesGeniusCalculator.Calculator.Infrastructure;
using BillGatesGeniusCalculator.MessageBox;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace BillGatesGeniusCalculator.Calculator.Unity.Views
{
    public sealed class CalculatorAppView : LifetimeScope
    {
        [SerializeField] private CalculatorAppViewConfig appConfig = new CalculatorAppViewConfig();
        [SerializeField] private CalculatorScreenViewConfig screenConfig = new CalculatorScreenViewConfig();
        [SerializeField] private MessageBoxViewConfig messageBoxConfig = new MessageBoxViewConfig();
        [SerializeField] private CalculatorScreenView screenView;
        [SerializeField] private MessageBoxView messageBoxView;

        public void Initialize(CalculatorAppViewConfig config)
        {
            appConfig = config;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(appConfig);
            builder.RegisterInstance(screenConfig);
            builder.RegisterInstance(messageBoxConfig);
            builder.RegisterInstance(screenView).As<ICalculatorScreenView>();
            builder.RegisterInstance(messageBoxView).As<IMessageBoxView>();
            builder.Register<AdditionExpressionEvaluator>(Lifetime.Singleton).As<IExpressionEvaluator>();
            builder.Register(_ => new UnityJsonStateRepository(), Lifetime.Singleton).As<IStateRepository>();
            builder.Register<MessageBoxPresenter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CalculatorPresenter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterEntryPoint<CalculatorStartup>();
        }
    }
}
