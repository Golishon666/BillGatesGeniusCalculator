using System.Collections.Generic;
using System.Text;
using BillGatesGeniusCalculator.Calculator.Application;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BillGatesGeniusCalculator.Calculator.Unity.Views
{
    public sealed class CalculatorScreenView : MonoBehaviour, ICalculatorScreenView
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI placeholderText;
        [SerializeField] private Image inputLineImage;
        [SerializeField] private Button resultButton;
        [SerializeField] private TextMeshProUGUI resultButtonLabel;
        [SerializeField] private RectTransform historyContent;
        [SerializeField] private TextMeshProUGUI historyText;
        [SerializeField] private ScrollRect historyScrollRect;
        [SerializeField] private Scrollbar historyScrollbar;
        [SerializeField] private int maxVisibleHistoryRows = 7;
        [SerializeField] private string historyLineIndent = "      ";
        [SerializeField] private Color inputLineDefaultColor = new(0.8039216f, 0.8039216f, 0.8039216f, 1f);
        [SerializeField] private Color inputLineActiveColor = new(0.29803923f, 0.68235296f, 0.93333334f, 1f);
        [SerializeField] private float resultButtonHistoryOffset;

        private readonly StringBuilder _historyBuilder = new StringBuilder();
        private RectTransform _panelRect;
        private RectTransform _historyScrollRectTransform;
        private RectTransform _resultButtonRect;
        private ICalculatorScreenPresenter _presenter;
        private bool _isSettingInput;
        private bool _baseLayoutCaptured;
        private Vector2 _basePanelSize;
        private Vector2 _basePanelAnchoredPosition;
        private float _basePanelTopWorldY;
        private Vector2 _baseResultButtonAnchoredPosition;
        private Vector2 _baseHistoryScrollSize;
        private float _historyRowHeight;
        private Image _historyViewportImage;

        public void Initialize(ICalculatorScreenPresenter presenter, CalculatorScreenViewConfig config)
        {
            CacheRects();
            ConfigureHistoryScrollRect();
            EnsureHistoryViewportRaycast();
            CaptureBaseLayout();
            ApplyHistoryLayout(0f);
            UpdateInputLineAppearance(false);
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

        private void Update()
        {
            if (_presenter == null || inputField.isFocused)
            {
                return;
            }

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _presenter.ClearHistoryDebug();
            }
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

                    _historyBuilder.Append(historyLineIndent);
                    _historyBuilder.Append(line);
                    historyCount++;
                }
            }

            historyText.text = _historyBuilder.ToString();
            var hasHistory = historyCount > 0;
            var hasScrollableHistory = historyCount > maxVisibleHistoryRows;
            var visibleHistoryHeight = Mathf.Min(historyCount, maxVisibleHistoryRows) * _historyRowHeight;
            var contentHeight = hasScrollableHistory
                ? GetHistoryContentHeight()
                : visibleHistoryHeight;
            ApplyHistoryLayout(visibleHistoryHeight);
            historyContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, hasHistory ? contentHeight : 0f);
            Canvas.ForceUpdateCanvases();
            if (hasHistory)
            {
                historyScrollRect.verticalNormalizedPosition = 1f;
            }

            UpdateHistoryScrollState(hasScrollableHistory);
            UpdateInputLineAppearance(hasHistory);
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
            _historyScrollRectTransform = (RectTransform)historyScrollRect.transform;
            _resultButtonRect = (RectTransform)resultButton.transform;
        }

        private void CaptureBaseLayout()
        {
            if (_baseLayoutCaptured)
            {
                return;
            }

            CacheRects();
            _basePanelSize = _panelRect.sizeDelta;
            _basePanelAnchoredPosition = _panelRect.anchoredPosition;
            _basePanelTopWorldY = GetRectTopWorldY(_panelRect);
            _baseResultButtonAnchoredPosition = _resultButtonRect.anchoredPosition;
            _baseHistoryScrollSize = _historyScrollRectTransform.sizeDelta;
            _historyRowHeight = Mathf.Max(1f, historyText.GetPreferredValues("0").y);
            _baseLayoutCaptured = true;
        }

        private void ApplyHistoryLayout(float visibleHistoryHeight)
        {
            CaptureBaseLayout();

            var hasHistoryLayout = visibleHistoryHeight > 0f;
            var layoutExpansion = hasHistoryLayout
                ? visibleHistoryHeight + resultButtonHistoryOffset
                : 0f;

            _panelRect.sizeDelta = new Vector2(_basePanelSize.x, _basePanelSize.y + layoutExpansion);
            _panelRect.anchoredPosition = _basePanelAnchoredPosition;
            RestorePanelTopWorldY();

            _historyScrollRectTransform.gameObject.SetActive(hasHistoryLayout);
            _historyScrollRectTransform.sizeDelta = new Vector2(_baseHistoryScrollSize.x, visibleHistoryHeight);

            _resultButtonRect.anchoredPosition = new Vector2(
                _baseResultButtonAnchoredPosition.x,
                _baseResultButtonAnchoredPosition.y - layoutExpansion);
        }

        private void RestorePanelTopWorldY()
        {
            var parentRect = _panelRect.parent as RectTransform;
            if (parentRect == null)
            {
                return;
            }

            var topDelta = _basePanelTopWorldY - GetRectTopWorldY(_panelRect);
            if (Mathf.Approximately(topDelta, 0f))
            {
                return;
            }

            var localDelta = parentRect.InverseTransformVector(new Vector3(0f, topDelta, 0f));
            _panelRect.anchoredPosition += new Vector2(0f, localDelta.y);
        }

        private static float GetRectTopWorldY(RectTransform rect)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            return corners[1].y;
        }

        private void ConfigureHistoryScrollRect()
        {
            historyScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            historyScrollRect.horizontal = false;
            historyScrollRect.vertical = false;
            historyScrollRect.verticalScrollbar = null;

            if (historyScrollbar != null)
            {
                historyScrollbar.gameObject.SetActive(false);
            }
        }

        private void EnsureHistoryViewportRaycast()
        {
            var viewport = historyScrollRect.viewport;
            if (viewport == null)
            {
                return;
            }

            if (!viewport.TryGetComponent<Image>(out _historyViewportImage))
            {
                _historyViewportImage = viewport.gameObject.AddComponent<Image>();
                _historyViewportImage.color = Color.clear;
            }

            _historyViewportImage.raycastTarget = false;
        }

        private float GetHistoryContentHeight()
        {
            if (string.IsNullOrEmpty(historyText.text))
            {
                return 0f;
            }

            return historyText.GetPreferredValues(historyText.text, _baseHistoryScrollSize.x, 0f).y;
        }

        private void UpdateHistoryScrollState(bool isScrollable)
        {
            historyScrollRect.vertical = isScrollable;
            historyScrollRect.verticalScrollbar = isScrollable ? historyScrollbar : null;

            if (_historyViewportImage != null)
            {
                _historyViewportImage.raycastTarget = isScrollable;
            }

            if (historyScrollbar == null)
            {
                return;
            }

            historyScrollbar.gameObject.SetActive(isScrollable);
        }

        private void UpdateInputLineAppearance(bool hasHistory)
        {
            if (inputLineImage == null)
            {
                return;
            }

            inputLineImage.color = hasHistory ? inputLineActiveColor : inputLineDefaultColor;
        }
    }
}
