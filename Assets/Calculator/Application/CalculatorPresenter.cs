using BillGatesGeniusCalculator.Calculator.Domain;
using BillGatesGeniusCalculator.MessageBox;
using Cysharp.Threading.Tasks;

namespace BillGatesGeniusCalculator.Calculator.Application
{
    public sealed class CalculatorPresenter : ICalculatorPresenter, ICalculatorScreenPresenter
    {
        private const string ErrorToken = "ERROR";

        private readonly ICalculatorScreenView _view;
        private readonly IExpressionEvaluator _evaluator;
        private readonly IStateRepository _stateRepository;
        private readonly IMessageDialogService _messageDialogService;
        private readonly CalculatorScreenViewConfig _config;

        private CalculatorState _state = new CalculatorState();
        private bool _isInitialized;

        public CalculatorPresenter(
            ICalculatorScreenView view,
            IExpressionEvaluator evaluator,
            IStateRepository stateRepository,
            IMessageDialogService messageDialogService,
            CalculatorScreenViewConfig config)
        {
            _view = view;
            _evaluator = evaluator;
            _stateRepository = stateRepository;
            _messageDialogService = messageDialogService;
            _config = config;
        }

        public async UniTask InitializeAsync()
        {
            _state = await _stateRepository.LoadAsync();
            _view.SetInput(_state.CurrentInput);
            _view.SetHistory(_state.History);
            _view.SetInteractable(true);
            _isInitialized = true;
        }

        public void OnInputChanged(string input)
        {
            if (!_isInitialized)
            {
                return;
            }

            _state.CurrentInput = input ?? string.Empty;
            SaveAsync().Forget();
        }

        public void OnResultRequested(string input)
        {
            HandleResultAsync(input ?? string.Empty).Forget();
        }

        public void ClearHistoryDebug()
        {
            if (!_isInitialized)
            {
                return;
            }

            _state.History.Clear();
            _view.SetHistory(_state.History);
            SaveAsync().Forget();
        }

        private async UniTaskVoid HandleResultAsync(string input)
        {
            if (!_isInitialized)
            {
                return;
            }

            _view.SetInteractable(false);
            var evaluation = _evaluator.Evaluate(input);

            if (evaluation.IsSuccess)
            {
                _state.History.Insert(0, $"{input}={evaluation.Value}");
                _state.CurrentInput = string.Empty;
                _view.SetInput(_state.CurrentInput);
                _view.SetHistory(_state.History);
                await _stateRepository.SaveAsync(_state);
                _view.SetInteractable(true);
                return;
            }

            _state.History.Insert(0, $"{input}={ErrorToken}");
            _state.CurrentInput = string.Empty;
            _view.SetInput(_state.CurrentInput);
            _view.SetHistory(_state.History);
            await _stateRepository.SaveAsync(_state);
            _view.Hide();
            await _messageDialogService.ShowAsync(_config.ErrorDialogMessage, _config.ErrorDialogButtonText);
            _view.Show();
            _state.CurrentInput = input;
            _view.SetInput(_state.CurrentInput);
            _view.SetInteractable(true);
            await _stateRepository.SaveAsync(_state);
        }

        private async UniTaskVoid SaveAsync()
        {
            await _stateRepository.SaveAsync(_state);
        }
    }
}
