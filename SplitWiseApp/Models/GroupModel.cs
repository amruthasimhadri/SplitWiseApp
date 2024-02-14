namespace SplitWiseApp.Models
{
    public class GroupModel
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int CreatedBy { get; set; }
        public int TypeId { get; set; } // Ensure this 
    }
}
