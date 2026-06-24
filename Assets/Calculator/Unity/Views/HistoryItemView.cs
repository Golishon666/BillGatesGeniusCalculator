using UnityEngine;
using UnityEngine.UI;

namespace BillGatesGeniusCalculator.Calculator.Unity.Views
{
    public sealed class HistoryItemView : MonoBehaviour
    {
        [SerializeField] private Text label;

        public void Initialize(string text)
        {
            label.text = text;
        }
    }
}
