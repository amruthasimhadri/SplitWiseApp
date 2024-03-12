namespace SplitWiseApp.Models
{
    public class ExpenseGroupViewModel
    {
        public int ExpenseId { get; set; }

        public decimal TotalAmount { get;set; }//ad
        public decimal OverallOwed { get; set; }
        public Dictionary<string, decimal> IndividualOwedAmounts { get; set; }
        public string PaidByName { get; set; }
        public decimal PaidAmount { get; set; }
         public string Description { get; set; }
        public int GroupId { get; set; }

        public decimal TotalGroupAmount { get; set; }
    }
}
