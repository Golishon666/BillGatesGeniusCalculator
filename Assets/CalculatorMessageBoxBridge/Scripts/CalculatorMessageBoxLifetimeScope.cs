using BillGatesGeniusCalculator.Calculator.Unity.Views;
using BillGatesGeniusCalculator.MessageBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BillGatesGeniusCalculator.CalculatorMessageBoxBridge
{
    public sealed class CalculatorMessageBoxLifetimeScope : LifetimeScope
    {
        [SerializeField] private CalculatorAppView calculatorScope;
        [SerializeField] private MessageBoxLifetimeScope messageBoxScope;

        public CalculatorAppView CalculatorScope => calculatorScope;

        public MessageBoxLifetimeScope MessageBoxScope => messageBoxScope;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(new CalculatorMessageBoxScopeReferences(
                calculatorScope,
                messageBoxScope,
                this));
            builder.RegisterEntryPoint<CalculatorMessageBoxStartup>();
        }
    }

    public sealed class CalculatorMessageBoxScopeReferences
    {
        public CalculatorMessageBoxScopeReferences(
            CalculatorAppView calculatorScope,
            MessageBoxLifetimeScope messageBoxScope,
            Object context)
        {
            CalculatorScope = calculatorScope;
            MessageBoxScope = messageBoxScope;
            Context = context;
        }

        public CalculatorAppView CalculatorScope { get; }

        public MessageBoxLifetimeScope MessageBoxScope { get; }

        public Object Context { get; }
    }
}
