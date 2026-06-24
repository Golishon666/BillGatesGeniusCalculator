using Cysharp.Threading.Tasks;

namespace BillGatesGeniusCalculator.Calculator.Application
{
    public delegate UniTask MessageDialogRequestedHandler(string message, string buttonText);

    public sealed class MessageDialogEventService : IMessageDialogService
    {
        public event MessageDialogRequestedHandler MessageDialogRequested;

        public async UniTask ShowAsync(string message, string buttonText)
        {
            var handlers = MessageDialogRequested;
            if (handlers == null)
            {
                return;
            }

            foreach (MessageDialogRequestedHandler handler in handlers.GetInvocationList())
            {
                await handler(message, buttonText);
            }
        }
    }
}
