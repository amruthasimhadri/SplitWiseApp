using Microsoft.AspNetCore.Mvc.Rendering;

namespace SplitWiseApi
{
    public class AddExpense
    {
        public int GroupId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int PaidBy { get; set; }
        public List<SelectListItem>? groupMembers { get; set; }
        public AddExpense()
        {
            groupMembers = new List<SelectListItem>();
        }
        public string memberId { get; set; }
    }

    public class AddExpenseToDB
    {
        public int GroupId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int PaidBy { get; set; }
    }
}
