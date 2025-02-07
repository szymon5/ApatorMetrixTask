using Moq;
using ApatorMetrixTask.Interfaces;
using ApatorMetrixTask.ViewModel;
using System.Windows;
using ApatorMetrixTask.Models;
using Org.BouncyCastle.Asn1.X509;

namespace ApatorMetrixTask.Test
{
    [TestFixture]
    public class PaymentCardManagementSystemViewModel_Test
    {
        private Mock<IRepository> _repository;
        private Mock<IMessageBoxService> _messageBoxService;
        private PaymentCardManagementSystemViewModel pcmsVM;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IRepository>();
            _messageBoxService = new Mock<IMessageBoxService>();
            pcmsVM = new PaymentCardManagementSystemViewModel(_repository.Object, _messageBoxService.Object);
        }

        [Test]
        public async Task AddNewPaymentCardCommand_AfterExecuting_NewPaymentCardMustBeInTheDatabase()
        {
            pcmsVM.OwnerAccountNumber = "1231123";
            pcmsVM.Pin = "1234";
            pcmsVM.CardSerialNumber = "12121";

            var initialCount = pcmsVM.PaymentCards.Count;

            _repository.Setup(repo => repo.AddNewPaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(true);

            await Task.Run(() => pcmsVM.AddNewPaymentCardCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.PaymentCards.Count, Is.EqualTo(initialCount + 1));

                _messageBoxService.Verify(service =>
                    service.ShowWithButtonAndImage("New payment card has been added.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information), Times.Once);
            });
        }

        [Test]
        public async Task AddNewPaymentCardCommand_AfterExecuting_ShouldReturnNotEnoughtDataProvidedErrorMessageBox()
        {
            pcmsVM.OwnerAccountNumber = "1231123123";
            pcmsVM.Pin = "1234";
            pcmsVM.CardSerialNumber = "";

            var initialCount = pcmsVM.PaymentCards.Count;

            _repository.Setup(repo => repo.AddNewPaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(true);

            await Task.Run(() => pcmsVM.AddNewPaymentCardCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.PaymentCards.Count, Is.EqualTo(initialCount));

                _messageBoxService.Verify(service =>
                    service.ShowWithButtonAndImage("You have not provided all required data.", "Failed", 
                    MessageBoxButton.OK, MessageBoxImage.Error), Times.Once);
            });
        }

        [Test]
        public async Task FindPaymentCardCommand_OnlyOneParamProvided_AfterExecuting_ListShouldNotBeEmpty()
        {
            pcmsVM.OwnerAccountNumber = "12345";

            var expectedCards = new List<PaymentCard>
            {
                new PaymentCard()
                {
                    OwnerAccountNumber = pcmsVM.OwnerAccountNumber,
                }
            };

            _repository.Setup(repo => repo.FindPaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(expectedCards);

            await Task.Run(() => pcmsVM.FindPaymentCardCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(expectedCards.Count, Is.EqualTo(pcmsVM.PaymentCards.Count));
                Assert.That(pcmsVM.PaymentCardFound, Is.EqualTo(Visibility.Visible));
                Assert.That(pcmsVM.NoPaymentCardFoundInfo, Is.EqualTo(Visibility.Collapsed));
            });
        }

        [Test]
        public async Task FindPaymentCardCommand_TwoParamsProvided_AfterExecuting_ListShouldNotBeEmpty()
        {
            pcmsVM.OwnerAccountNumber = "12345";
            pcmsVM.CardSerialNumber = "1222";

            var expectedCards = new List<PaymentCard>
            {
                new PaymentCard()
                {
                    OwnerAccountNumber = pcmsVM.OwnerAccountNumber,
                    CardSerialNumber = pcmsVM.CardSerialNumber,
                }
            };

            _repository.Setup(repo => repo.FindPaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(expectedCards);

            await Task.Run(() => pcmsVM.FindPaymentCardCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(expectedCards.Count, Is.EqualTo(pcmsVM.PaymentCards.Count));
                Assert.That(pcmsVM.PaymentCardFound, Is.EqualTo(Visibility.Visible));
                Assert.That(pcmsVM.NoPaymentCardFoundInfo, Is.EqualTo(Visibility.Collapsed));
            });
        }

        [Test]
        public async Task FindPaymentCardCommand_AllParamsProvided_AfterExecuting_ListShouldNotBeEmpty()
        {
            pcmsVM.OwnerAccountNumber = "12345";
            pcmsVM.CardSerialNumber = "1222";
            pcmsVM.UCID = "e6f7e06d99e34bfb9d154eb9823eb48e";

            var expectedCards = new List<PaymentCard>
            {
                new PaymentCard()
                {
                    OwnerAccountNumber = pcmsVM.OwnerAccountNumber,
                    CardSerialNumber = pcmsVM.CardSerialNumber,
                    UCID = pcmsVM.UCID,
                }
            };

            _repository.Setup(repo => repo.FindPaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(expectedCards);

            await Task.Run(() => pcmsVM.FindPaymentCardCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(expectedCards.Count, Is.EqualTo(pcmsVM.PaymentCards.Count));
                Assert.That(pcmsVM.PaymentCardFound, Is.EqualTo(Visibility.Visible));
                Assert.That(pcmsVM.NoPaymentCardFoundInfo, Is.EqualTo(Visibility.Collapsed));
            });
        }

        [Test]
        public async Task FindPaymentCardCommand_ParamsProvidedButNothingFounded_AfterExecuting_ListShouldBeEmpty()
        {
            pcmsVM.OwnerAccountNumber = "hehehehehe";
            pcmsVM.CardSerialNumber = "hehehehe";
            pcmsVM.UCID = "hehehe";

            var expectedCards = new List<PaymentCard>
            {
                new PaymentCard()
                {
                    OwnerAccountNumber = pcmsVM.OwnerAccountNumber,
                    CardSerialNumber = pcmsVM.CardSerialNumber,
                    UCID = pcmsVM.UCID,
                }
            };

            _repository.Setup(repo => repo.FindPaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(new List<PaymentCard>());

            await Task.Run(() => pcmsVM.FindPaymentCardCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.PaymentCards.Count, Is.EqualTo(0));
                Assert.That(pcmsVM.PaymentCardFound, Is.EqualTo(Visibility.Collapsed));
                Assert.That(pcmsVM.NoPaymentCardFoundInfo, Is.EqualTo(Visibility.Visible));
            });
        }

        [Test]
        public async Task FindPaymentCardCommand_NoParamsProvided_AfterExecuting_ListShouldNotBeEmpty()
        {
            var expectedCards = new List<PaymentCard>
            {
                new PaymentCard()
                {
                    OwnerAccountNumber = pcmsVM.OwnerAccountNumber,
                    CardSerialNumber = pcmsVM.CardSerialNumber,
                    UCID = pcmsVM.UCID,
                }
            };

            _repository.Setup(repo => repo.FindPaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(new List<PaymentCard>());

            await Task.Run(() => pcmsVM.FindPaymentCardCommand.Execute(null));

            _messageBoxService.Verify(service =>
                    service.ShowWithButtonAndImage("You have to provide Owner account number, card serial number or unique card ID to find a card.", "Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error), Times.Once);
        }

        [Test]
        public async Task GetAllPaymentCardsCommand_AfterExecuting_ShouldReturnList()
        {
            var expectedCards = new List<PaymentCard>
            {
                new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "111111", Pin = "1234", CardSerialNumber = "45454545", UCID = "121212", CardNumber = "989898989", ExpiryDate = "2025-01-01", CVV = "123" },
                new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "3333", Pin = "5678", CardSerialNumber = "222222", UCID = "5484848", CardNumber = "56565656", ExpiryDate = "2030-03-01", CVV = "456" },
                new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "444444444", Pin = "9090", CardSerialNumber = "111111111", UCID = "21231233", CardNumber = "88888", ExpiryDate = "2029-06-01", CVV = "789" },
            };

            _repository.Setup(repo => repo.GetAllPaymentCardsAsync())
                       .ReturnsAsync(expectedCards);

            await Task.Run(() => pcmsVM.GetAllPaymentCardsCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.PaymentCards.Count, Is.EqualTo(expectedCards.Count));
                Assert.That(pcmsVM.PaymentCardFound, Is.EqualTo(Visibility.Visible));
                Assert.That(pcmsVM.NoPaymentCardFoundInfo, Is.EqualTo(Visibility.Collapsed));
            });
        }

        [Test]
        public async Task GetAllPaymentCardsCommand_AfterExecuting_ShouldReturnAnEmptyList()
        {
            _repository.Setup(repo => repo.GetAllPaymentCardsAsync())
                       .ReturnsAsync(new List<PaymentCard>());

            await Task.Run(() => pcmsVM.GetAllPaymentCardsCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.PaymentCards.Count, Is.EqualTo(0));
                Assert.That(pcmsVM.PaymentCardFound, Is.EqualTo(Visibility.Collapsed));
                Assert.That(pcmsVM.NoPaymentCardFoundInfo, Is.EqualTo(Visibility.Visible));
            });
        }

        [Test]
        public async Task RemovePaymentCardCommand_AfterClickYes_RemovedElementMustNotBeInTheList()
        {
            pcmsVM.PaymentCards = new System.Collections.ObjectModel.ObservableCollection<PaymentCard>
            {
                new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "111111", Pin = "1234", CardSerialNumber = "45454545", UCID = "121212", CardNumber = "989898989", ExpiryDate = "2025-01-01", CVV = "123" },
                new PaymentCard { PaymentCardID = 2, OwnerAccountNumber = "333", Pin = "5678", CardSerialNumber = "222222", UCID = "5484848", CardNumber = "56565656", ExpiryDate = "2030-03-01", CVV = "456" },
                new PaymentCard { PaymentCardID = 3, OwnerAccountNumber = "4444444", Pin = "9090", CardSerialNumber = "111111111", UCID = "21231233", CardNumber = "88888", ExpiryDate = "2029-06-01", CVV = "789" },
            };

            var cardToRemove = pcmsVM.PaymentCards.FirstOrDefault(c => c.OwnerAccountNumber == "111111" && c.CardSerialNumber == "45454545" && c.UCID == "121212");

            Assert.NotNull(cardToRemove, "Card to remove not found in the list");

            pcmsVM.OwnerAccountNumber = cardToRemove.OwnerAccountNumber;
            pcmsVM.CardSerialNumber = cardToRemove.CardSerialNumber;
            pcmsVM.UCID = cardToRemove.UCID;

            int initialQuantity = pcmsVM.PaymentCards.Count;

            _messageBoxService.Setup(service => service.ShowWithButtonAndImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>()))
                              .Returns(MessageBoxResult.Yes);

            _repository.Setup(repo => repo.RemovePaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(true);

            await Task.Run(() => pcmsVM.RemovePaymentCardCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.PaymentCards.Count, Is.EqualTo(initialQuantity - 1));

                Assert.That(pcmsVM.PaymentCards.Any(card =>
                    card.PaymentCardID == cardToRemove.PaymentCardID), Is.False, "Karta powinna zostaæ usuniêta.");

                _messageBoxService.Verify(service =>
                    service.ShowWithButtonAndImage("The payment card has been removed.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information), Times.Once);
            });
        }

        [Test]
        public async Task RemovePaymentCardCommand_AfterClickNo_RemovedElementMustBeInTheList()
        {
            pcmsVM.PaymentCards = new System.Collections.ObjectModel.ObservableCollection<PaymentCard>
            {
                new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "111111", Pin = "1234", CardSerialNumber = "45454545", UCID = "121212", CardNumber = "989898989", ExpiryDate = "2025-01-01", CVV = "123" },
                new PaymentCard { PaymentCardID = 2, OwnerAccountNumber = "333", Pin = "5678", CardSerialNumber = "222222", UCID = "5484848", CardNumber = "56565656", ExpiryDate = "2030-03-01", CVV = "456" },
                new PaymentCard { PaymentCardID = 3, OwnerAccountNumber = "4444444", Pin = "9090", CardSerialNumber = "111111111", UCID = "21231233", CardNumber = "88888", ExpiryDate = "2029-06-01", CVV = "789" },
            };

            var cardToRemove = pcmsVM.PaymentCards.FirstOrDefault(c => c.OwnerAccountNumber == "111111" && c.CardSerialNumber == "45454545" && c.UCID == "121212");

            Assert.NotNull(cardToRemove, "Card to remove not found in the list");

            pcmsVM.OwnerAccountNumber = cardToRemove.OwnerAccountNumber;
            pcmsVM.CardSerialNumber = cardToRemove.CardSerialNumber;
            pcmsVM.UCID = cardToRemove.UCID;

            int initialQuantity = pcmsVM.PaymentCards.Count;

            _messageBoxService.Setup(service => service.ShowWithButtonAndImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>()))
                              .Returns(MessageBoxResult.No);

            _repository.Setup(repo => repo.RemovePaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(false);

            await Task.Run(() => pcmsVM.RemovePaymentCardCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.PaymentCards.Count, Is.EqualTo(initialQuantity));

                Assert.That(pcmsVM.PaymentCards.Any(card =>
                    card.PaymentCardID == cardToRemove.PaymentCardID), Is.True);
            });
        }

        [Test]
        public async Task RemovePaymentCardCommand_AfterClickCancel_RemovedElementMustBeInTheList()
        {
            pcmsVM.PaymentCards = new System.Collections.ObjectModel.ObservableCollection<PaymentCard>
            {
                new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "111111", Pin = "1234", CardSerialNumber = "45454545", UCID = "121212", CardNumber = "989898989", ExpiryDate = "2025-01-01", CVV = "123" },
                new PaymentCard { PaymentCardID = 2, OwnerAccountNumber = "333", Pin = "5678", CardSerialNumber = "222222", UCID = "5484848", CardNumber = "56565656", ExpiryDate = "2030-03-01", CVV = "456" },
                new PaymentCard { PaymentCardID = 3, OwnerAccountNumber = "4444444", Pin = "9090", CardSerialNumber = "111111111", UCID = "21231233", CardNumber = "88888", ExpiryDate = "2029-06-01", CVV = "789" },
            };

            var cardToRemove = pcmsVM.PaymentCards.FirstOrDefault(c => c.OwnerAccountNumber == "111111" && c.CardSerialNumber == "45454545" && c.UCID == "121212");

            Assert.NotNull(cardToRemove, "Card to remove not found in the list");

            pcmsVM.OwnerAccountNumber = cardToRemove.OwnerAccountNumber;
            pcmsVM.CardSerialNumber = cardToRemove.CardSerialNumber;
            pcmsVM.UCID = cardToRemove.UCID;

            int initialQuantity = pcmsVM.PaymentCards.Count;

            _messageBoxService.Setup(service => service.ShowWithButtonAndImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>()))
                              .Returns(MessageBoxResult.Cancel);

            _repository.Setup(repo => repo.RemovePaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(false);

            await Task.Run(() => pcmsVM.RemovePaymentCardCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.PaymentCards.Count, Is.EqualTo(initialQuantity));
                Assert.That(pcmsVM.PaymentCards.Any(card => card.PaymentCardID == cardToRemove.PaymentCardID), Is.True);
            });
        }

        [Test]
        public async Task RemovePaymentCardCommand_AfterExecuting_NoDataProvided_ShouldReturnError()
        {
            pcmsVM.OwnerAccountNumber = "qqq";
            pcmsVM.CardSerialNumber = "qqq";
            pcmsVM.UCID = "qqq";

            _messageBoxService.Setup(service => service.ShowWithButtonAndImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>()))
                              .Returns(MessageBoxResult.Yes);

            _repository.Setup(repo => repo.RemovePaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(false);

            await Task.Run(() => pcmsVM.RemovePaymentCardCommand.Execute(null));

            _messageBoxService.Verify(service =>
                    service.ShowWithButtonAndImage("Payment card with such data doesnot exist.", "Failed", 
                    MessageBoxButton.OK, MessageBoxImage.Error), Times.Once);
        }

        [Test]
        public async Task RemovePaymentCardCommand_AfterExecuting_ShouldReturnError()
        {
            pcmsVM.OwnerAccountNumber = "";
            pcmsVM.CardSerialNumber = "";
            pcmsVM.UCID = "";

            _messageBoxService.Setup(service => service.ShowWithButtonAndImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>()))
                              .Returns(MessageBoxResult.Yes);

            _repository.Setup(repo => repo.RemovePaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(false);

            await Task.Run(() => pcmsVM.RemovePaymentCardCommand.Execute(null));

            _messageBoxService.Verify(service =>
                    service.ShowWithButtonAndImage("You have not provided all required data.", "Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error), Times.Once);
        }

        [Test]
        public void ResetVariables_AfterExecuting_TheVariablesShouldBeEmpty()
        {
            pcmsVM.OwnerAccountNumber = "qwqwqw";
            pcmsVM.Pin = "qwqwq";
            pcmsVM.CardSerialNumber = "qwqwq";
            pcmsVM.UCID = "qwqwqwq";

            pcmsVM.ResetVariablesCommand.Execute(null);

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.OwnerAccountNumber, Is.EqualTo(string.Empty));
                Assert.That(pcmsVM.Pin, Is.EqualTo(string.Empty));
                Assert.That(pcmsVM.CardSerialNumber, Is.EqualTo(string.Empty));
                Assert.That(pcmsVM.UCID, Is.EqualTo(string.Empty));
            });
        }

        [Test]
        public void LoadWastebinIcon_AfterExecute_ImageMustBeLoaded()
        {
            pcmsVM.WastebinIcon = null;

            pcmsVM.LoadEmbeddedImageCommand.Execute(null);

            Assert.IsNotNull(pcmsVM.WastebinIcon);
        }

        [Test]
        public async Task RemovePaymentCardFromListCommand_AfterClickYes_RemovedElementMustNotBeInTheList()
        {
            var cardToRemove = new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "111111", Pin = "1234", CardSerialNumber = "45454545", UCID = "121212", CardNumber = "989898989", ExpiryDate = "2025-01-01", CVV = "123" };

            pcmsVM.PaymentCards.Add(cardToRemove);

            int initialQuantity = pcmsVM.PaymentCards.Count;

            _messageBoxService.Setup(service => service.ShowWithButtonAndImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>()))
                              .Returns(MessageBoxResult.Yes);

            _repository.Setup(repo => repo.RemovePaymentFromListCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(true);

            await Task.Run(() => pcmsVM.RemovePaymentCardFromListCommand.Execute(cardToRemove));

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.PaymentCards.Count, Is.EqualTo(initialQuantity - 1));
                Assert.That(pcmsVM.PaymentCards.Any(card => card.PaymentCardID == cardToRemove.PaymentCardID), Is.False);

                _messageBoxService.Verify(service =>
                    service.ShowWithButtonAndImage("The payment card has been removed.", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information), Times.Once);
            });
        }

        [Test]
        public async Task RemovePaymentCardFromListCommand_AfterClickCancel_RemovedElementMustBeInTheList()
        {
            pcmsVM.PaymentCards = new System.Collections.ObjectModel.ObservableCollection<PaymentCard>
            {
                new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "111111", Pin = "1234", CardSerialNumber = "45454545", UCID = "121212", CardNumber = "989898989", ExpiryDate = "2025-01-01", CVV = "123" },
                new PaymentCard { PaymentCardID = 2, OwnerAccountNumber = "333", Pin = "5678", CardSerialNumber = "222222", UCID = "5484848", CardNumber = "56565656", ExpiryDate = "2030-03-01", CVV = "456" },
                new PaymentCard { PaymentCardID = 3, OwnerAccountNumber = "4444444", Pin = "9090", CardSerialNumber = "111111111", UCID = "21231233", CardNumber = "88888", ExpiryDate = "2029-06-01", CVV = "789" },
            };

            var cardToRemove = pcmsVM.PaymentCards.FirstOrDefault(c => c.OwnerAccountNumber == "111111" && c.CardSerialNumber == "45454545" && c.UCID == "121212");

            Assert.NotNull(cardToRemove, "Card to remove not found in the list");

            pcmsVM.OwnerAccountNumber = cardToRemove.OwnerAccountNumber;
            pcmsVM.CardSerialNumber = cardToRemove.CardSerialNumber;
            pcmsVM.UCID = cardToRemove.UCID;

            int initialQuantity = pcmsVM.PaymentCards.Count;

            _messageBoxService.Setup(service => service.ShowWithButtonAndImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>()))
                              .Returns(MessageBoxResult.Cancel);

            _repository.Setup(repo => repo.RemovePaymentCardAsync(It.IsAny<PaymentCard>()))
                       .ReturnsAsync(false);

            await Task.Run(() => pcmsVM.RemovePaymentCardCommand.Execute(null));

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.PaymentCards.Count, Is.EqualTo(initialQuantity));
                Assert.That(pcmsVM.PaymentCards.Any(card => card.PaymentCardID == cardToRemove.PaymentCardID), Is.True);
            });
        }

        [Test]
        public async Task SelectPaymentCardFromList_AfterClick_IsSelectedMustBeTrue()
        {
            var paymentCard = new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "111111", Pin = "1234", CardSerialNumber = "1212121212121", UCID = "90cc20d9cc934cc5aefde3c22432d9e6", CardNumber = "9999 9999 9999 9999", ExpiryDate = "2025-01-01", CVV = "123", IsSelected = false };
            var cardHolder = new PaymentCardOwner { PaymentCardOwnerID = 1, PaymentCardID = 1, Name = "Andrzej", Surname = "Kowalski" };

            string cardHolderFullName = cardHolder.OwnerFullName;

            _repository.Setup(repo => repo.GetPaymentCardHolderAsync(It.IsAny<int>()))
                       .ReturnsAsync(cardHolderFullName);

            await Task.Run(() => pcmsVM.SelectPaymentCardFromListCommand.Execute(paymentCard));

            Assert.Multiple(() =>
            {
                Assert.That(paymentCard.IsSelected, Is.True);
                Assert.That(pcmsVM.BankName, Is.EqualTo("Bank name.."));
                Assert.That(pcmsVM.CardNumber, Is.EqualTo(paymentCard.CardNumber));
                Assert.That(pcmsVM.CardHolder, Is.EqualTo(cardHolder.OwnerFullName));
                Assert.That(pcmsVM.ExpiryDateMonth, Is.EqualTo(paymentCard.ExpiryDateMonth));
                Assert.That(pcmsVM.ExpiryDateYear, Is.EqualTo(paymentCard.ExpiryDateYear));
                Assert.That(pcmsVM.CVV, Is.EqualTo(paymentCard.CVV));
            });
        }

        [Test]
        public async Task SelectPaymentCardFromList_AfterClick_IsSelectedMustBeFalse()
        {
            var paymentCard = new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "111111", Pin = "1234", CardSerialNumber = "45454545", UCID = "121212", CardNumber = "9999 9999 9999 9999", ExpiryDate = "2025-01-01", CVV = "123", IsSelected = true };

            await Task.Run(() => pcmsVM.SelectPaymentCardFromListCommand.Execute(paymentCard));

            Assert.Multiple(() =>
            {
                Assert.That(paymentCard.IsSelected, Is.False);
                Assert.That(pcmsVM.BankName, Is.EqualTo("Bank name.."));
                Assert.That(pcmsVM.CardNumber, Is.EqualTo("1234 1234 1234 1234"));
                Assert.That(pcmsVM.CardHolder, Is.EqualTo("Jan Nowak"));
                Assert.That(pcmsVM.ExpiryDateMonth, Is.EqualTo("03"));
                Assert.That(pcmsVM.ExpiryDateYear, Is.EqualTo("26"));
                Assert.That(pcmsVM.CVV, Is.EqualTo("123"));
            });
        }

        [Test]
        public async Task SelectPaymentCardFromList_AfterClick_ReturnNoCardHolder()
        {
            var paymentCard = new PaymentCard { PaymentCardID = 1, OwnerAccountNumber = "111111", Pin = "1234", CardSerialNumber = "45454545", UCID = "121212", CardNumber = "9999 9999 9999 9999", ExpiryDate = "2025-01-01", CVV = "123", IsSelected = false };

            string defaulCardHolderFullName = "Jan Nowak";

            await Task.Run(() => pcmsVM.SelectPaymentCardFromListCommand.Execute(paymentCard));

            Assert.Multiple(() =>
            {
                Assert.That(paymentCard.IsSelected, Is.True);
                Assert.That(pcmsVM.BankName, Is.EqualTo("Bank name.."));
                Assert.That(pcmsVM.CardNumber, Is.EqualTo(paymentCard.CardNumber));
                Assert.That(pcmsVM.CardHolder, Is.EqualTo(defaulCardHolderFullName));
                Assert.That(pcmsVM.ExpiryDateMonth, Is.EqualTo(paymentCard.ExpiryDateMonth));
                Assert.That(pcmsVM.ExpiryDateYear, Is.EqualTo(paymentCard.ExpiryDateYear));
                Assert.That(pcmsVM.CVV, Is.EqualTo(paymentCard.CVV));
            });
        }

        [Test]
        public void InitCard_AfterExecute_SHouldHaveDefaultValues()
        {
            // No Arrange is required here

            pcmsVM.InitCardCommand.Execute(null);

            Assert.Multiple(() =>
            {
                Assert.That(pcmsVM.BankName, Is.EqualTo("Bank name.."));
                Assert.That(pcmsVM.CardNumber, Is.EqualTo("1234 1234 1234 1234"));
                Assert.That(pcmsVM.CardHolder, Is.EqualTo("Jan Nowak"));
                Assert.That(pcmsVM.ExpiryDateMonth, Is.EqualTo("03"));
                Assert.That(pcmsVM.ExpiryDateYear, Is.EqualTo("26"));
                Assert.That(pcmsVM.CVV, Is.EqualTo("123"));
            });
        }

        [Test]
        public void ProducePaymentCardFromList_AfterClick_SuccessMessageBoxMustBeDisplayed()
        {
            pcmsVM.BankName = "MBANK";
            pcmsVM.CardNumber = "1234 1234 1234 1234";
            pcmsVM.CardHolder = "Jan Nowak";
            pcmsVM.ExpiryDateMonth = "03";
            pcmsVM.ExpiryDateYear = "26";
            pcmsVM.CVV = "123";

            pcmsVM.ProducePaymentCardCommand.Execute("True");

            _messageBoxService.Verify(service =>
                    service.ShowWithButtonAndImage("The VISA payment card can be produced.", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information), Times.Once);
        }

        [Test]
        public void ProducePaymentCardFromList_AfterClick_ErrorMessageBoxMustBeDisplayed()
        {
            pcmsVM.BankName = "";
            pcmsVM.CardNumber = "1234123412341234";
            pcmsVM.CardHolder = "Jan Nowak";
            pcmsVM.ExpiryDateMonth = "03";
            pcmsVM.ExpiryDateYear = "26";
            pcmsVM.CVV = "123";

            pcmsVM.ProducePaymentCardCommand.Execute("True");

            _messageBoxService.Verify(service =>
                    service.ShowWithButtonAndImage("ERROR! Check the data again", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error), Times.Once);
        }
    }
}