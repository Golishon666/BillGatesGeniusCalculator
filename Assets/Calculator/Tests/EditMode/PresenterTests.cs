using System.Collections.Generic;
using System.Threading.Tasks;
using BillGatesGeniusCalculator.Calculator.Application;
using BillGatesGeniusCalculator.Calculator.Domain;
using BillGatesGeniusCalculator.MessageBox;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace BillGatesGeniusCalculator.Calculator.Tests
{
    public sealed class PresenterTests
    {
        [Test]
        public async Task InitializeAsync_RestoresSavedInputAndHistory()
        {
            var state = new CalculatorState("34+47", new[] { "5+5=10" });
            var view = new FakeCalculatorScreenView();
            var presenter = CreatePresenter(view, new FakeStateRepository(state), new FakeDialogService());

            await presenter.InitializeAsync();

            Assert.That(view.Input, Is.EqualTo("34+47"));
            Assert.That(view.History, Is.EqualTo(new[] { "5+5=10" }));
            Assert.That(view.IsInteractable, Is.True);
        }

        [Test]
        public async Task OnResultRequested_AddsSuccessHistoryAndClearsInput()
        {
            var repository = new FakeStateRepository(new CalculatorState());
            var view = new FakeCalculatorScreenView();
            var presenter = CreatePresenter(view, repository, new FakeDialogService());
            await presenter.InitializeAsync();

            presenter.OnResultRequested("5+5");
            await UniTask.DelayFrame(2);

            Assert.That(view.Input, Is.Empty);
            Assert.That(view.History, Is.EqualTo(new[] { "5+5=10" }));
            Assert.That(repository.SavedState.CurrentInput, Is.Empty);
            Assert.That(repository.SavedState.History, Is.EqualTo(new[] { "5+5=10" }));
        }

        [Test]
        public async Task OnResultRequested_ShowsDialogAndRestoresInput_OnError()
        {
            var repository = new FakeStateRepository(new CalculatorState());
            var dialog = new FakeDialogService();
            var view = new FakeCalculatorScreenView();
            var presenter = CreatePresenter(view, repository, dialog);
            await presenter.InitializeAsync();

            presenter.OnResultRequested("5/5");
            await UniTask.DelayFrame(2);

            Assert.That(dialog.ShowCount, Is.EqualTo(1));
            Assert.That(view.Input, Is.EqualTo("5/5"));
            Assert.That(view.History, Is.EqualTo(new[] { "5/5=ERROR" }));
            Assert.That(repository.SavedState.CurrentInput, Is.EqualTo("5/5"));
        }

        [Test]
        public async Task OnInputChanged_SavesInput()
        {
            var repository = new FakeStateRepository(new CalculatorState());
            var view = new FakeCalculatorScreenView();
            var presenter = CreatePresenter(view, repository, new FakeDialogService());
            await presenter.InitializeAsync();

            presenter.OnInputChanged("10+20");
            await UniTask.DelayFrame(1);

            Assert.That(repository.SavedState.CurrentInput, Is.EqualTo("10+20"));
        }

        private static CalculatorPresenter CreatePresenter(
            FakeCalculatorScreenView view,
            IStateRepository repository,
            IMessageDialogService dialogService)
        {
            return new CalculatorPresenter(
                view,
                new AdditionExpressionEvaluator(),
                repository,
                dialogService,
                new CalculatorScreenViewConfig());
        }

        private sealed class FakeCalculatorScreenView : ICalculatorScreenView
        {
            public string Input { get; private set; }
            public List<string> History { get; } = new List<string>();
            public bool IsInteractable { get; private set; }

            public void Initialize(ICalculatorScreenPresenter presenter, CalculatorScreenViewConfig config)
            {
            }

            public void SetInput(string input)
            {
                Input = input;
            }

            public void SetHistory(IReadOnlyList<string> history)
            {
                History.Clear();
                History.AddRange(history);
            }

            public void SetInteractable(bool isInteractable)
            {
                IsInteractable = isInteractable;
            }
        }

        private sealed class FakeStateRepository : IStateRepository
        {
            private readonly CalculatorState _loadState;

            public FakeStateRepository(CalculatorState loadState)
            {
                _loadState = loadState;
            }

            public CalculatorState SavedState { get; private set; }

            public UniTask<CalculatorState> LoadAsync()
            {
                return UniTask.FromResult(new CalculatorState(_loadState.CurrentInput, _loadState.History));
            }

            public UniTask SaveAsync(CalculatorState state)
            {
                SavedState = new CalculatorState(state.CurrentInput, state.History);
                return UniTask.CompletedTask;
            }
        }

        private sealed class FakeDialogService : IMessageDialogService
        {
            public int ShowCount { get; private set; }

            public UniTask ShowAsync(string message, string buttonText)
            {
                ShowCount++;
                return UniTask.CompletedTask;
            }
        }
    }
}
