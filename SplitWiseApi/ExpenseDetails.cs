namespace SplitWiseApi
{
    public class ExpenseDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; }
        public string GroupName { get; set; }
        public int PaidBy { get; set; }
        public int MemberId { get; set; }
        public int ExpenseId { get; set; }//added

        public decimal TotalAmount { get;set; }
        public string ExpenseDescription { get; set; }//added
    }
}
