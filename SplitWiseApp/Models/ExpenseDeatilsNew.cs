namespace SplitWiseApp.Models
{
    public class ExpenseDeatilsNew
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; }
        public string GroupName { get; set; }
        public int PaidBy { get; set; }
        public int MemberId { get; set; }
        public int ExpenseId { get; set; }
        public string ExpenseDescription { get; set; }
    }
}
