namespace ApatorMetrixTask.Models
{
    public class PaymentCardOwner
    {
        public int PaymentCardOwnerID { get; set; }
        public int PaymentCardID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string OwnerFullName => $"{Name} {Surname}";
    }
}
