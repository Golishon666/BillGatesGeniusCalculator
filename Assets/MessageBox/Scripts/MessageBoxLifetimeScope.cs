using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BillGatesGeniusCalculator.MessageBox
{
    public sealed class MessageBoxLifetimeScope : LifetimeScope
    {
        [SerializeField] private MessageBoxViewConfig messageBoxConfig = new MessageBoxViewConfig();
        [SerializeField] private MessageBoxView messageBoxView;

        private MessageBoxPresenter _presenter;

        public UniTask ShowAsync(string message, string buttonText)
        {
            if (_presenter == null)
            {
                Debug.LogError($"{nameof(MessageBoxLifetimeScope)} is not initialized.", this);
                return UniTask.CompletedTask;
            }

            return _presenter.ShowAsync(message, buttonText);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            messageBoxView ??= GetComponentInChildren<MessageBoxView>(true);
            builder.RegisterInstance(messageBoxConfig);

            if (messageBoxView == null)
            {
                Debug.LogError($"{nameof(MessageBoxView)} is not assigned for {nameof(MessageBoxLifetimeScope)}.", this);
                return;
            }

            _presenter = new MessageBoxPresenter(messageBoxView);
            builder.RegisterInstance(messageBoxView).As<IMessageBoxView>();
            builder.RegisterInstance(_presenter).As<IMessageBoxPresenter>();
            builder.RegisterEntryPoint<MessageBoxStartup>();
        }
    }
}
