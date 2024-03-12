namespace SplitWiseApp.Models
{
    public class PaymentModel
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
    }

    
    public class OwedAmountModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public decimal AmountOwed { get; set; }
    }
}
