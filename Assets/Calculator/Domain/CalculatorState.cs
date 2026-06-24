using System;
using System.Collections.Generic;

namespace BillGatesGeniusCalculator.Calculator.Domain
{
    [Serializable]
    public sealed class CalculatorState
    {
        public CalculatorState()
            : this(string.Empty, Array.Empty<string>())
        {
        }

        public CalculatorState(string currentInput, IReadOnlyList<string> history)
        {
            CurrentInput = currentInput ?? string.Empty;
            History = new List<string>(history ?? Array.Empty<string>());
        }

        public string CurrentInput { get; set; }
        public List<string> History { get; }
    }
}
