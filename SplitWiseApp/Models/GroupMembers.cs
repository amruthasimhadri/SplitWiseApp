﻿namespace SplitWiseApp.Models
{
    public class GroupMembers
    {
        public int GroupId { get; set; }
        public int FriendId { get; set; }
    }
    public class ExpenseMembers
    {
        public int ExpenseId { get; set; }
        public int FriendId { get; set; }
    }

    public class Friends
    {
       // public int Id { get; set; }
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public string FriendName { get; set; }

        public string FriendEmail { get; set; }
       // public bool IsSelected { get; set; }
    }

   
}
