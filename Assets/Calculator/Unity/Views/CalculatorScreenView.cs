using System.Collections.Generic;
using System.Text;
using BillGatesGeniusCalculator.Calculator.Application;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BillGatesGeniusCalculator.Calculator.Unity.Views
{
    public sealed class CalculatorScreenView : MonoBehaviour, ICalculatorScreenView
    {
        private const float HistoryRowHeight = 44f;
        private const int MaxVisibleHistoryRows = 6;
        private const string HistoryLineIndent = "      ";
        private const float BasePanelWidth = 720f;
        private const float BasePanelHeight = 480f;
        private const float BasePanelAnchoredY = -130f;
        private const float ContentWidth = 610f;
        private const float TitleCenterOffsetY = -70f;
        private const float InputCenterOffsetY = -165f;
        private const float InputLineCenterOffsetY = -242f;
        private const float HistoryTopOffsetY = -254f;
        private const float ResultButtonCenterOffsetY = -332f;
        private const float TitleHeight = 40f;
        private const float InputHeight = 70f;
        private const float InputLineHeight = 4f;
        private const float ResultButtonHeight = 96f;

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI placeholderText;
        [SerializeField] private RectTransform inputLine;
        [SerializeField] private Button resultButton;
        [SerializeField] private TextMeshProUGUI resultButtonLabel;
        [SerializeField] private RectTransform historyContent;
        [SerializeField] private TextMeshProUGUI historyText;
        [SerializeField] private ScrollRect historyScrollRect;
        [SerializeField] private Scrollbar historyScrollbar;

        private readonly StringBuilder _historyBuilder = new StringBuilder();
        private RectTransform _panelRect;
        private RectTransform _titleRect;
        private RectTransform _inputRect;
        private RectTransform _historyScrollRectTransform;
        private RectTransform _resultButtonRect;
        private ICalculatorScreenPresenter _presenter;
        private bool _isSettingInput;

        public void Initialize(ICalculatorScreenPresenter presenter, CalculatorScreenViewConfig config)
        {
            CacheRects();
            ApplyHistoryLayout(0);
            _presenter = presenter;
            titleText.text = config.Title;
            placeholderText.text = config.Placeholder;
            resultButtonLabel.text = config.ResultButtonText;
            inputField.onValueChanged.RemoveAllListeners();
            inputField.onValueChanged.AddListener(OnInputValueChanged);
            resultButton.onClick.RemoveAllListeners();
            resultButton.onClick.AddListener(() => _presenter.OnResultRequested(inputField.text));
            Show();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetInput(string input)
        {
            _isSettingInput = true;
            inputField.text = input ?? string.Empty;
            _isSettingInput = false;
        }

        public void SetHistory(IReadOnlyList<string> history)
        {
            _historyBuilder.Clear();
            var historyCount = 0;
            if (history != null)
            {
                foreach (var line in history)
                {
                    if (historyCount > 0)
                    {
                        _historyBuilder.AppendLine();
                    }

                    _historyBuilder.Append(HistoryLineIndent);
                    _historyBuilder.Append(line);
                    historyCount++;
                }
            }

            historyText.text = _historyBuilder.ToString();
            var hasHistory = historyCount > 0;
            var hasScrollableHistory = historyCount > MaxVisibleHistoryRows;
            var visibleHistoryHeight = Mathf.Min(historyCount, MaxVisibleHistoryRows) * HistoryRowHeight;
            var contentHeight = historyCount * HistoryRowHeight;
            ApplyHistoryLayout(visibleHistoryHeight);
            historyContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, hasHistory ? contentHeight : 0f);
            Canvas.ForceUpdateCanvases();
            if (hasHistory)
            {
                historyScrollRect.verticalNormalizedPosition = 1f;
            }

            UpdateHistoryScrollbar(hasScrollableHistory, visibleHistoryHeight, contentHeight);
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

        private void CacheRects()
        {
            _panelRect = (RectTransform)transform;
            _titleRect = titleText.rectTransform;
            _inputRect = (RectTransform)inputField.transform;
            _historyScrollRectTransform = (RectTransform)historyScrollRect.transform;
            _resultButtonRect = (RectTransform)resultButton.transform;
        }

        private void ApplyHistoryLayout(float visibleHistoryHeight)
        {
            CacheRects();

            var hasHistory = visibleHistoryHeight > 0f;
            var panelHeight = BasePanelHeight + visibleHistoryHeight;
            _panelRect.sizeDelta = new Vector2(BasePanelWidth, panelHeight);
            _panelRect.anchoredPosition = new Vector2(0f, BasePanelAnchoredY - visibleHistoryHeight * 0.5f);

            SetTopAnchoredRect(_titleRect, ContentWidth, TitleHeight, TitleCenterOffsetY, new Vector2(0.5f, 0.5f));
            SetTopAnchoredRect(_inputRect, ContentWidth, InputHeight, InputCenterOffsetY, new Vector2(0.5f, 0.5f));
            SetTopAnchoredRect(inputLine, ContentWidth, InputLineHeight, InputLineCenterOffsetY, new Vector2(0.5f, 0.5f));
            SetTopAnchoredRect(_resultButtonRect, ContentWidth, ResultButtonHeight, ResultButtonCenterOffsetY - visibleHistoryHeight, new Vector2(0.5f, 0.5f));

            _historyScrollRectTransform.gameObject.SetActive(hasHistory);
            SetTopAnchoredRect(_historyScrollRectTransform, ContentWidth, visibleHistoryHeight, HistoryTopOffsetY, new Vector2(0.5f, 1f));
            ConfigureStretch(historyContent, new Vector2(0.5f, 1f));
            ConfigureStretch(historyText.rectTransform, new Vector2(0.5f, 1f));
        }

        private void UpdateHistoryScrollbar(bool isVisible, float visibleHistoryHeight, float contentHeight)
        {
            if (historyScrollbar == null)
            {
                return;
            }

            historyScrollbar.gameObject.SetActive(isVisible);
            if (!isVisible)
            {
                return;
            }

            historyScrollbar.size = contentHeight > 0f ? Mathf.Clamp01(visibleHistoryHeight / contentHeight) : 1f;
            historyScrollbar.value = 1f;
        }

        private static void SetTopAnchoredRect(RectTransform rect, float width, float height, float anchoredY, Vector2 pivot)
        {
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = pivot;
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(0f, anchoredY);
        }

        private static void ConfigureStretch(RectTransform rect, Vector2 pivot)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = pivot;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}


