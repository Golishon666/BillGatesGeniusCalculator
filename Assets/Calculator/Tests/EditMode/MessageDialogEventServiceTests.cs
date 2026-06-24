using System.Threading.Tasks;
using BillGatesGeniusCalculator.Calculator.Application;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace BillGatesGeniusCalculator.Calculator.Tests
{
    public sealed class MessageDialogEventServiceTests
    {
        [Test]
        public async Task ShowAsync_InvokesAndAwaitsSubscribers()
        {
            var service = new MessageDialogEventService();
            var wasAwaited = false;

            service.MessageDialogRequested += async (message, buttonText) =>
            {
                await UniTask.Yield();
                Assert.That(message, Is.EqualTo("Invalid expression"));
                Assert.That(buttonText, Is.EqualTo("OK"));
                wasAwaited = true;
            };

            await service.ShowAsync("Invalid expression", "OK");

            Assert.That(wasAwaited, Is.True);
        }

        [Test]
        public async Task ShowAsync_CompletesWithoutSubscribers()
        {
            var service = new MessageDialogEventService();

            await service.ShowAsync("Invalid expression", "OK");
        }
    }
}
