using System;
using BillGatesGeniusCalculator.Calculator.Unity.Views;
using BillGatesGeniusCalculator.MessageBox;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace BillGatesGeniusCalculator.CalculatorMessageBoxBridge
{
    public sealed class CalculatorMessageBoxStartup : IStartable, IDisposable
    {
        private readonly CalculatorMessageBoxScopeReferences _scopeReferences;
        private CalculatorAppView _calculatorScope;
        private MessageBoxLifetimeScope _messageBoxScope;

        public CalculatorMessageBoxStartup(CalculatorMessageBoxScopeReferences scopeReferences)
        {
            _scopeReferences = scopeReferences;
        }

        public void Start()
        {
            _calculatorScope = _scopeReferences.CalculatorScope;
            _messageBoxScope = _scopeReferences.MessageBoxScope;

            if (_calculatorScope == null || _messageBoxScope == null)
            {
                Debug.LogError(
                    $"{nameof(CalculatorMessageBoxLifetimeScope)} must reference calculator and message box scopes.",
                    _scopeReferences.Context);
                return;
            }

            _calculatorScope.MessageDialogRequested += ShowMessageAsync;
        }

        public void Dispose()
        {
            if (_calculatorScope != null)
            {
                _calculatorScope.MessageDialogRequested -= ShowMessageAsync;
            }
        }

        private UniTask ShowMessageAsync(string message, string buttonText)
        {
            return _messageBoxScope != null
                ? _messageBoxScope.ShowAsync(message, buttonText)
                : UniTask.CompletedTask;
        }
    }
}
