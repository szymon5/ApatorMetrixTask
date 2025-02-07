using ApatorMetrixTask.Commands;
using ApatorMetrixTask.Interfaces;
using ApatorMetrixTask.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ApatorMetrixTask.ViewModel
{
    public class PaymentCardManagementSystemViewModel : INotifyPropertyChanged
    {
        private readonly IRepository _repository;
        private readonly IMessageBoxService _messageBoxService;

        private string _ownerAccountNumber;
        private string _pin;
        private string _cardSerialNumber;
        private string _ucid;
        private Visibility _paymentCardFound;
        private Visibility _noPaymentCardFoundInfo;
        private ImageSource _wastebinIcon;
        private PaymentCard _selectedPaymentCard;
        private int _selectedTabControlIndex;
        private string _bankName;
        private string _cardNumber;
        private string _cardHolder;
        private string _expiryDateMonth;
        private string _expiryDateYear;
        private string _cvv;

        public string OwnerAccountNumber
        {
            get => _ownerAccountNumber;
            set { _ownerAccountNumber = value; OnPropertyChanged(); }
        }

        public string Pin
        {
            get => _pin;
            set { _pin = value; OnPropertyChanged(); }
        }

        public string CardSerialNumber
        {
            get => _cardSerialNumber;
            set { _cardSerialNumber = value; OnPropertyChanged(); }
        }

        public string UCID
        {
            get => _ucid;
            set { _ucid = value; OnPropertyChanged(); }
        }

        public Visibility PaymentCardFound
        {
            get { return _paymentCardFound; }
            set
            {
                if (_paymentCardFound != value)
                {
                    _paymentCardFound = value;
                    OnPropertyChanged(nameof(PaymentCardFound));
                }
            }
        }

        public Visibility NoPaymentCardFoundInfo
        {
            get { return _noPaymentCardFoundInfo; }
            set
            {
                if (_noPaymentCardFoundInfo != value)
                {
                    _noPaymentCardFoundInfo = value;
                    OnPropertyChanged(nameof(NoPaymentCardFoundInfo));
                }
            }
        }

        public ImageSource WastebinIcon
        {
            get => _wastebinIcon;
            set
            {
                _wastebinIcon = value;
                OnPropertyChanged(nameof(WastebinIcon));
            }
        }

        public PaymentCard SelectedPaymentCard
        {
            get => _selectedPaymentCard;
            private set
            {
                _selectedPaymentCard = value;
                OnPropertyChanged(nameof(SelectedPaymentCard));
            }
        }

        public int SelectedTabControlIndex
        {
            get => _selectedTabControlIndex;
            set
            {
                if (_selectedTabControlIndex != value)
                {
                    _selectedTabControlIndex = value;
                    OnPropertyChanged(nameof(SelectedTabControlIndex));
                }
            }
        }

        public string BankName
        {
            get => _bankName;
            set
            {
                if (_bankName != value)
                {
                    _bankName = value;
                    OnPropertyChanged(nameof(BankName));
                }
            }
        }

        public string CardNumber
        {
            get => _cardNumber;
            set
            {
                if (_cardNumber != value)
                {
                    if(value == "")
                    {
                        _cardNumber = value;
                        OnPropertyChanged(nameof(CardNumber));
                    }
                    else if(IsDigitOnly(value))
                    {
                        if (CountStringWithoutSpaces(value) <= 16)
                        {
                            _cardNumber = AddSpace(value);
                            OnPropertyChanged(nameof(CardNumber));
                        }
                    }
                }
            }
        }

        public string CardHolder
        {
            get => _cardHolder;
            set
            {
                if (_cardHolder != value)
                {
                    _cardHolder = value;
                    OnPropertyChanged(nameof(CardHolder));
                }
            }
        }

        public string ExpiryDateMonth
        {
            get => _expiryDateMonth;
            set
            {
                if (value == "" || IsDigitOnly(value))
                {
                    _expiryDateMonth = value;
                    OnPropertyChanged(nameof(CardNumber));
                }
            }
        }

        public string ExpiryDateYear
        {
            get => _expiryDateYear;
            set
            {
                if (_expiryDateYear != value)
                {
                    _expiryDateYear = value;
                    OnPropertyChanged(nameof(ExpiryDateYear));
                }
            }
        }

        public string CVV
        {
            get => _cvv;
            set
            {
                if (_cvv != value)
                {
                    _cvv = value;
                    OnPropertyChanged(nameof(CVV));
                }
            }
        }

        public ICommand AddNewPaymentCardCommand { get; }
        public ICommand FindPaymentCardCommand { get; }
        public ICommand GetAllPaymentCardsCommand { get; }
        public ICommand RemovePaymentCardCommand { get; }
        public ICommand ResetVariablesCommand { get; }
        public ICommand LoadEmbeddedImageCommand { get; }
        public ICommand RemovePaymentCardFromListCommand { get; }
        public ICommand SelectPaymentCardFromListCommand { get; }
        public ICommand InitCardCommand { get; }
        public ICommand ProducePaymentCardCommand { get; }

        public ObservableCollection<PaymentCard> PaymentCards { get; set; }

        public PaymentCardManagementSystemViewModel(IRepository repository, IMessageBoxService messageBoxService)
        {
            AddNewPaymentCardCommand = new AsyncRelayCommand(AddNewPaymentCard); 
            FindPaymentCardCommand = new AsyncRelayCommand(FindPaymentCard);
            GetAllPaymentCardsCommand = new AsyncRelayCommand(GetAllPaymentCards);
            RemovePaymentCardCommand = new AsyncRelayCommand(RemovePaymentCard);
            ResetVariablesCommand = new RelayCommand(ResetVariables);
            LoadEmbeddedImageCommand = new RelayCommand(LoadEmbeddedImage);
            RemovePaymentCardFromListCommand = new AsyncGenericRelayCommand<PaymentCard>(RemovePaymentCardFromList);
            SelectPaymentCardFromListCommand = new AsyncGenericRelayCommand<PaymentCard>(SelectPaymentCardFromList);
            InitCardCommand = new RelayCommand(InitCard);
            ProducePaymentCardCommand = new GenericRelayCommand<string>(ProducePaymentCard);

            PaymentCardFound = Visibility.Collapsed;
            NoPaymentCardFoundInfo = Visibility.Collapsed;

            PaymentCards = new ObservableCollection<PaymentCard>();

            LoadEmbeddedImage();

            _repository = repository;
            _messageBoxService = messageBoxService;
        }

        private async Task AddNewPaymentCard()
        {
            if (!string.IsNullOrWhiteSpace(OwnerAccountNumber) &&
                !string.IsNullOrWhiteSpace(Pin) &&
                !string.IsNullOrWhiteSpace(CardSerialNumber))
            {
                var newPaymentCard = new PaymentCard
                {
                    OwnerAccountNumber = OwnerAccountNumber,
                    Pin = Pin,
                    CardSerialNumber = CardSerialNumber,
                    UCID = Guid.NewGuid().ToString("N"),
                };

                if (await _repository.AddNewPaymentCardAsync(newPaymentCard))
                {
                    PaymentCards.Add(newPaymentCard);
                    OwnerAccountNumber = string.Empty;
                    Pin = string.Empty;
                    CardSerialNumber = string.Empty;

                    _messageBoxService.ShowWithButtonAndImage("New payment card has been added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else _messageBoxService.ShowWithButtonAndImage("Something went wrong. You have not added a new payment card.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else _messageBoxService.ShowWithButtonAndImage("You have not provided all required data.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async Task FindPaymentCard()
        {
            if (!string.IsNullOrWhiteSpace(OwnerAccountNumber) ||
                !string.IsNullOrWhiteSpace(CardSerialNumber) ||
                !string.IsNullOrWhiteSpace(UCID))
            {
                var list = await _repository.FindPaymentCardAsync(new PaymentCard
                {
                    OwnerAccountNumber = OwnerAccountNumber,
                    CardSerialNumber = CardSerialNumber,
                    UCID = UCID,
                });

                PaymentCards.Clear();

                for(int i=0;i<list.Count;i++) PaymentCards.Add(list[i]);

                PaymentCardFound = PaymentCards.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                NoPaymentCardFoundInfo = PaymentCards.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else _messageBoxService.ShowWithButtonAndImage("You have to provide Owner account number, card serial number or unique card ID to find a card.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async Task GetAllPaymentCards()
        {
            var list = await _repository.GetAllPaymentCardsAsync();

            PaymentCards.Clear();

            for (int i = 0; i < list.Count; i++) PaymentCards.Add(list[i]);

            PaymentCardFound = PaymentCards.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            NoPaymentCardFoundInfo = PaymentCards.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task RemovePaymentCard()
        {
            if (!string.IsNullOrWhiteSpace(OwnerAccountNumber) &&
                !string.IsNullOrWhiteSpace(CardSerialNumber) &&
                !string.IsNullOrWhiteSpace(UCID))
            {
                var decision = _messageBoxService.ShowWithButtonAndImage("Are you sure you want to remove this payment card? This operation cannot be undone.", "Remove", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if(decision == MessageBoxResult.Yes)
                {
                    var paymentCard = PaymentCards.FirstOrDefault(pc => pc.OwnerAccountNumber.Equals(OwnerAccountNumber) && pc.CardSerialNumber.Equals(CardSerialNumber) && pc.UCID.Equals(UCID));

                    if(paymentCard != null)
                    {
                        if(await _repository.RemovePaymentCardAsync(paymentCard))
                        {
                            OwnerAccountNumber = string.Empty;
                            UCID = string.Empty;
                            CardSerialNumber = string.Empty;

                            PaymentCards.Remove(paymentCard);

                            _messageBoxService.ShowWithButtonAndImage("The payment card has been removed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else _messageBoxService.ShowWithButtonAndImage("Payment card with such data doesnot exist.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else _messageBoxService.ShowWithButtonAndImage("You have not provided all required data.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ResetVariables()
        {
            OwnerAccountNumber = string.Empty;
            Pin = string.Empty;
            CardSerialNumber = string.Empty;
            UCID = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadEmbeddedImage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("ApatorMetrixTask.Icons.wastebin.jpg"))
            {
                if (stream == null) return;
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                WastebinIcon = image;
            }
        }

        private async Task RemovePaymentCardFromList(PaymentCard paymentCard)
        {
            var decision = _messageBoxService.ShowWithButtonAndImage("Are you sure you want to remove this payment card? This operation cannot be undone.", "Remove", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            if (decision == MessageBoxResult.Yes)
            {
                if(await _repository.RemovePaymentFromListCardAsync(paymentCard))
                {
                    PaymentCards.Remove(paymentCard);
                    _messageBoxService.ShowWithButtonAndImage("The payment card has been removed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private async Task SelectPaymentCardFromList(PaymentCard paymentCard)
        {
            foreach (var card in PaymentCards) card.IsSelected = false;

            paymentCard.IsSelected = !paymentCard.IsSelected;

            if (paymentCard.IsSelected)
            {
                var cardHolder = await _repository.GetPaymentCardHolderAsync(paymentCard.PaymentCardID);

                BankName = "Bank name..";
                CardNumber = paymentCard.CardNumber;
                CardHolder = string.IsNullOrEmpty(cardHolder) ? "Jan Nowak" : cardHolder;
                ExpiryDateMonth = paymentCard.ExpiryDateMonth;
                ExpiryDateYear = paymentCard.ExpiryDateYear;
                CVV = paymentCard.CVV;
            }
            else InitCard();
        }

        private void InitCard()
        {
            BankName = "Bank name..";
            CardNumber = "1234123412341234";
            CardHolder = "Jan Nowak";
            ExpiryDateMonth = "03";
            ExpiryDateYear = "26";
            CVV = "123";
        }

        private string AddSpace(string text)
        {
            text = text.Replace(" ", "");

            var formattedText = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0 && i % 4 == 0)
                    formattedText += " ";
                formattedText += text[i];
            }

            return formattedText;
        }

        private int CountStringWithoutSpaces(string text)
        {
            int count = 0;

            for(int i=0; i < text.Length; i++)
            {
                if (text[i] >= 48 && text[i] <= 57) count++;
            }

            return count;
        }

        private bool IsDigitOnly(string text) => text.Replace(" ", "").All(x => x >= 48 && x <= 57);

        private void ProducePaymentCard(string visa)
        {
            if (!BankName.Equals("") &&
               !CardNumber.Equals("") && CardNumber.Length == 19 &&
               !CardHolder.Equals("") &&
               !ExpiryDateMonth.Equals("") && (Convert.ToInt16(ExpiryDateMonth) > 0 && Convert.ToInt16(ExpiryDateMonth) <= 12) &&
               !ExpiryDateYear.Equals("") &&
               !CVV.Equals(""))
            {
                var card = bool.Parse(visa);
                _messageBoxService.ShowWithButtonAndImage($"The {(card ? "VISA" : "MasterCard")} payment card can be produced.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else _messageBoxService.ShowWithButtonAndImage("ERROR! Check the data again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
