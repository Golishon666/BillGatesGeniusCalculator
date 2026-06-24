using System.Collections.Generic;
using BillGatesGeniusCalculator.Calculator.Application;
using UnityEngine;
using UnityEngine.UI;

namespace BillGatesGeniusCalculator.Calculator.Unity.Views
{
    public sealed class CalculatorScreenView : MonoBehaviour, ICalculatorScreenView
    {
        [SerializeField] private RectTransform panel;
        [SerializeField] private Text titleText;
        [SerializeField] private InputField inputField;
        [SerializeField] private Text placeholderText;
        [SerializeField] private Button resultButton;
        [SerializeField] private Text resultButtonLabel;
        [SerializeField] private RectTransform historyContent;
        [SerializeField] private ScrollRect historyScrollRect;
        [SerializeField] private HistoryItemView historyItemPrefab;

        private readonly List<HistoryItemView> _historyItems = new List<HistoryItemView>();
        private ICalculatorScreenPresenter _presenter;
        private bool _isSettingInput;

        public void Initialize(ICalculatorScreenPresenter presenter, CalculatorScreenViewConfig config)
        {
            _presenter = presenter;
            titleText.text = config.Title;
            placeholderText.text = config.Placeholder;
            resultButtonLabel.text = config.ResultButtonText;
            inputField.onValueChanged.RemoveAllListeners();
            inputField.onValueChanged.AddListener(OnInputValueChanged);
            resultButton.onClick.RemoveAllListeners();
            resultButton.onClick.AddListener(() => _presenter.OnResultRequested(inputField.text));
        }

        public void SetInput(string input)
        {
            _isSettingInput = true;
            inputField.text = input ?? string.Empty;
            _isSettingInput = false;
        }

        public void SetHistory(IReadOnlyList<string> history)
        {
            foreach (var item in _historyItems)
            {
                Destroy(item.gameObject);
            }

            _historyItems.Clear();

            if (history != null)
            {
                foreach (var line in history)
                {
                    var item = Instantiate(historyItemPrefab, historyContent);
                    item.Initialize(line);
                    _historyItems.Add(item);
                }
            }

            ResizePanel(_historyItems.Count);
            Canvas.ForceUpdateCanvases();
            historyScrollRect.verticalNormalizedPosition = 1f;
        }

        public void SetInteractable(bool isInteractable)
        {
            inputField.interactable = isInteractable;
            resultButton.interactable = isInteractable;
        }

        private void OnInputValueChanged(string value)
        {
            if (_isSettingInput)
            {
                return;
            }

            _presenter.OnInputChanged(value);
        }

        private void ResizePanel(int historyCount)
        {
            var visibleRows = Mathf.Clamp(historyCount, 0, 6);
            var targetHeight = historyCount == 0 ? 300f : 330f + visibleRows * 44f;
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
        }
    }
}
