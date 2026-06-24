using System;

namespace BillGatesGeniusCalculator.MessageBox
{
    [Serializable]
    public sealed class MessageBoxViewConfig
    {
        public string DefaultMessage = "Please check the expression\nyou just entered";
        public string ConfirmButtonText = "GOT IT";
    }
}
