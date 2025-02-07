using System.ComponentModel;

namespace ApatorMetrixTask.Models
{
    public class PaymentCard : INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public int PaymentCardID { get; set; }
        public string OwnerAccountNumber { get; set; }
        public string Pin {  get; set; }
        public string CardSerialNumber { get; set; }

        /// <summary>
        /// Unikalny losowy alfanumeryczny identyfikator karty o zadanej długości (np. 32 znaki)
        /// </summary>
        public string UCID { get; set; }

        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }

        public string ExpiryDateMonth => Convert.ToDateTime(ExpiryDate).ToString("MM");
        public string ExpiryDateYear => Convert.ToDateTime(ExpiryDate).ToString("yy");

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
