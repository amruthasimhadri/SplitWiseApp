namespace SplitWiseApp.Models
{
    public class Expenses
    {
        public int GroupId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int paidBy { get; set; }
    }
}
