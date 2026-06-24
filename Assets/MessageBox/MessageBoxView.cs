using UnityEngine;
using UnityEngine.UI;

namespace BillGatesGeniusCalculator.MessageBox
{
    public sealed class MessageBoxView : MonoBehaviour, IMessageBoxView
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Text confirmButtonLabel;

        private IMessageBoxPresenter _presenter;

        public void Initialize(IMessageBoxPresenter presenter, MessageBoxViewConfig config)
        {
            _presenter = presenter;
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => _presenter.OnConfirmRequested());
            SetMessage(config.DefaultMessage);
            SetButtonText(config.ConfirmButtonText);
            Hide();
        }

        public void Show()
        {
            root.SetActive(true);
        }

        public void Hide()
        {
            root.SetActive(false);
        }

        public void SetMessage(string message)
        {
            messageText.text = message;
        }

        public void SetButtonText(string buttonText)
        {
            confirmButtonLabel.text = buttonText;
        }
    }
}
