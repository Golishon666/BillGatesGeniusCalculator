using BillGatesGeniusCalculator.Calculator.Application;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BillGatesGeniusCalculator.Calculator.Unity.Views
{
    public sealed class CalculatorHistoryDebugView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        private ICalculatorScreenPresenter _presenter;

        public void Initialize(ICalculatorScreenPresenter presenter)
        {
            _presenter = presenter;
        }

        private void Update()
        {
            if (_presenter == null || inputField == null || inputField.isFocused)
            {
                return;
            }

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _presenter.ClearHistoryDebug();
            }
        }
    }
}
