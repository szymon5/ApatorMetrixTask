using ApatorMetrixTask.Models;

namespace ApatorMetrixTask.Interfaces
{
    public interface IRepository
    {
        public Task<bool> AddNewPaymentCardAsync(PaymentCard paymentCard);
        public Task<List<PaymentCard>> FindPaymentCardAsync(PaymentCard paymentCard);
        public Task<List<PaymentCard>> GetAllPaymentCardsAsync();
        public Task<bool> RemovePaymentCardAsync(PaymentCard paymentCard);
        public Task<bool> RemovePaymentFromListCardAsync(PaymentCard paymentCard);
        public Task<string> GetPaymentCardHolderAsync(int paymentCardID);
    }
}
