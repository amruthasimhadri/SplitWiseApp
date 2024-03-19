using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SplitWiseApp.Models;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SplitWiseApp.Controllers
{
    [SessionCheck]
    public class SplitWiseController : Controller
    {
        private readonly ApiService _apiService;
        public SplitWiseController(ApiService apiService)
        {
            _apiService = apiService;
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            _apiService.AddUser(user);
            ViewBag.SuccessMessage = "Registered Successfully";
            return View();

        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginModel user)
        {
            int? response = _apiService.VerifyUser(user);

            if (response != null)
            {
                int userId = (int)response.Value;
                HttpContext.Session.SetInt32("UserId", userId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password";
                return View();
            }

        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        //----------------------Dashboard--------------------------------
        public IActionResult Dashboard()
        {
            int UserId = HttpContext.Session.GetInt32("UserId") ?? -1;
            var groups = _apiService.GetAllGroups(UserId);
            return View(groups);
           
            
        }
        public IActionResult GetOwedAmounts()
        {
            int UserId = HttpContext.Session.GetInt32("UserId") ?? -1;
            int groupId = HttpContext.Session.GetInt32("NewGroupId") ?? -1;
            List<ExpenseDetails> expenseDetails = _apiService.GetExpenseOfGroup(groupId,UserId);

            decimal totalOwedToMe = 0;
            decimal totalOwedByMe = 0;
            string groupName = "";
            string expenseDescription = "";

            foreach (var expense in expenseDetails)
            {
                groupName=expense.GroupName;
                expenseDescription = expense.ExpenseDescription;
                if (expense.PaidBy == UserId && !expense.Paid)
                {
                    totalOwedToMe += expense.Amount;
                }
                else if (expense.MemberId == UserId && !expense.Paid)
                {
                    totalOwedByMe += expense.Amount;
                }
            }
            //ViewBag.GroupName = groupName;
            //ViewBag.ExpenseDescription = expenseDescription;
            //ViewBag.TotalOwedToMe = totalOwedToMe;
            //ViewBag.TotalOwedByMe = totalOwedByMe;
            var model = new OwedAmountsViewModel
            {
                GroupName = groupName,
                TotalOwedToMe = totalOwedToMe,
                TotalOwedByMe = totalOwedByMe
            };
            // return View("GetOwedAmounts", model);
            return PartialView("GetOwedAmounts", model);
        }

        
        public IActionResult GetExpenseOfGroup(int groupId) //all expenses of the group
        {
            int UserId = HttpContext.Session.GetInt32("UserId") ?? -1;
            ViewBag.GroupId = groupId; // Pass groupId to the view
            HttpContext.Session.SetInt32("NewGroupId",groupId);
            List<ExpenseDetails> expenseDetails = _apiService.GetExpenseOfGroup(groupId,UserId);


            var groupedExpenses = expenseDetails.GroupBy(e => e.ExpenseId);

            // List to hold grouped expense details
            List<ExpenseGroupViewModel> expenseGroups = new List<ExpenseGroupViewModel>();

            foreach (var group in groupedExpenses)
            {
                decimal overallOwed = 0;
                Dictionary<string, decimal> individualOwedAmounts = new Dictionary<string, decimal>();
                string paidByName = ""; // Name of the person who paid for the expense
                decimal paidAmount = 0; // Total amount paid by the person who paid for the expense
                string Description = "";
                decimal TotalAmount = 0;
                // Get the ExpenseId from the group
                int expenseId = group.Key;
                decimal TotalGroupAmount = 0;

                foreach (var expense in group)
                {
                    TotalGroupAmount += expense.TotalAmount;
                    Description = expense.ExpenseDescription;
                    TotalAmount = expense.TotalAmount;
                    if (expense.PaidBy == expense.MemberId)
                    {
                        // If the PaidBy is the same as the MemberId, this is the person who paid for the expense
                        paidByName = expense.Name;
                        paidAmount += expense.Amount;
                        continue;
                    }

                    decimal owedAmount = expense.Paid ? expense.Amount : -expense.Amount;

                    overallOwed += owedAmount;

                    if (individualOwedAmounts.ContainsKey(expense.Name))
                    {
                        individualOwedAmounts[expense.Name] += owedAmount;
                    }
                    else
                    {
                        individualOwedAmounts.Add(expense.Name, owedAmount);
                    }
                }
                ViewBag.TotalGroupAmountSum = TotalGroupAmount; //added
                ViewBag.UserId = UserId;

                // Create view model for the expense group
                var expenseGroup = new ExpenseGroupViewModel
                {
                    ExpenseId = expenseId, // Assign the ExpenseId obtained from the group
                    OverallOwed = overallOwed,
                    IndividualOwedAmounts = individualOwedAmounts,
                    PaidByName = paidByName,
                    PaidAmount = paidAmount,
                    GroupId= groupId,
                    Description=Description,
                    TotalAmount=TotalAmount,
                    //TotalGroupAmount=TotalGroupAmount,
                };

                expenseGroups.Add(expenseGroup);
            }
           
            return View("GetExpenseOfGroup", expenseGroups);


        }

        //------------------------Create Group----------------------
        public IActionResult CreateGroup()
        {
            var categories = _apiService.GetGroupTypes();
            // ViewBag.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public IActionResult CreateGroup(GroupModel group)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;

            AddNewGroup newGroup = new AddNewGroup();
            newGroup.CreatedBy = userId;
            newGroup.GroupName = group.GroupName;
            newGroup.TypeId = group.TypeId; // Assign TypeId here

            int? groupId = _apiService.AddGroup(newGroup);

            if (groupId != null)
            {
                HttpContext.Session.SetInt32("GroupId", groupId.Value);
                ViewBag.SuccessMessage = "Group created successfully!";
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to create group.";
            }

            
            return RedirectToAction("GetFriends");

        }
        
        public IActionResult GetFriendsToAddInGroup(int groupId) // redirects after add group members button
        {
            ViewBag.GroupId = groupId;  
            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;
            var friends = _apiService.GetFriendsToAddInGroup(userId,groupId);
            return View("GetFriends", friends);
        }
        public IActionResult GetFriends(int groupId)// redirects friends after creating new group button
        {
            int GroupId= HttpContext.Session.GetInt32("GroupId") ?? -1;
            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;
            var friends = _apiService.GetFriendsToAddInGroup(userId, GroupId);
            return View(friends);
        }

        [HttpPost]
        public IActionResult AddGroupMembers([FromForm] List<int> friends)
        {
            int groupId = HttpContext.Session.GetInt32("GroupId") ?? -1;
            if (friends != null && friends.Any())
            {
                foreach (var friendId in friends)
                {
                    GroupMembers groupMember = new GroupMembers
                    {
                        GroupId = groupId,
                        FriendId = friendId
                    };
                    _apiService.AddGroupMember(groupMember);
                }
                ViewBag.SuccessMessage = "Group members added successfully!";
            }
            else
            {
                ViewBag.ErrorMessage = "No friends selected to add to the group.";
            }
            //return View ();
           // return RedirectToAction("AddExpense", new { groupId = groupId });
            return RedirectToAction("GetExpenseOfGroup", new { groupId = groupId });
        }
        //-------------------------Add Friend--------------------
        public IActionResult GetFriendsOfUser(int groupId)// layout top friends  to show friends of userId
        {
           // int GroupId = HttpContext.Session.GetInt32("GroupId") ?? -1;
            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;
            var friends = _apiService.GetFriendsOfUser(userId);
            return View(friends);
        }

        [HttpGet]
        public IActionResult AddFriend()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;
            var friends = _apiService.GetFriendsOfUser(userId);
            string? error = TempData["ErrorMessage"] as string;
            ViewBag.Error = error;
            return View(friends);
        }

        [HttpPost]
        public IActionResult AddFriend(Friends friends)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;
            friends.UserId = userId;
            int? response = _apiService.AddFriend(friends);
            

            if (response != null)
            {
                ViewBag.SuccessMessage = "Friend Added Sucessfully";
                return RedirectToAction("AddFriend");
            }
            else
            {
                // ViewBag.ErrorMessage = "No Person Found With The Email Entered";
                TempData["ErrorMessage"] = "No Person Found With The Email Entered";
                return RedirectToAction("AddFriend");
            }
        }
        
        //-----------Add Expense---------------------------
        [HttpGet]
        public IActionResult AddExpense(int groupId)
        {
            HttpContext.Session.SetInt32("GroupId", groupId);
            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;
            AddExpense expenseModel = _apiService.GetExpenseModel(groupId, userId);
            expenseModel.GroupId = groupId;
            HttpContext.Session.SetInt32("GroupIdOfExpense", groupId);
            return View(expenseModel);
        }
        [HttpPost]
        public IActionResult AddExpense(AddExpense expense)
        {
            AddExpenseToDB expense1 = new AddExpenseToDB();
            expense1.GroupId = expense.GroupId;
            expense1.Description = expense.Description;
            expense1.Amount = expense.Amount;
            expense1.PaidBy = Convert.ToInt32(expense.PaidBy);
            int? expenseId = _apiService.AddExpense(expense1);

            int MyExpenseId = expenseId.Value;
            HttpContext.Session.SetInt32("expenseId", MyExpenseId);

           // _apiService.AddExpenseMembers(expenseId.Value); ( commented by me)

            //(changed)  return RedirectToAction("ViewExpenseDetails", new { expenseId = expenseId.Value });
            return RedirectToAction("SelectExpenseMemebers");
        }
        //-----------------------------------------
        //------------------------Seelct Expense Memebers-------------
        [HttpGet]
        public IActionResult SelectExpenseMemebers(/*int groupId*/)
        {
             int groupId = HttpContext.Session.GetInt32("GroupId") ?? -1;

            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;
            var Expensefriends = _apiService.GetExpenseFriends(groupId, userId);
             return View("SelectExpenseMemebers", Expensefriends);
           // return RedirectToAction("AddExpeneMembers",Expensefriends);

        }
        [HttpPost]
        public IActionResult AddExpeneMembers([FromForm] List<int> friends) //selected expense members will be inserted into table
        {
            int expenseId = HttpContext.Session.GetInt32("expenseId") ?? -1;
            if (friends != null && friends.Any())
            {
                foreach (var friendId in friends)
                {
                    ExpenseMembers expenseMember = new ExpenseMembers
                    {
                        ExpenseId = expenseId,
                        FriendId = friendId
                    };
                    _apiService.AddExpenseMembers(expenseMember);
                }
                ViewBag.SuccessMessage = "Group members added successfully!";
                _apiService.AddExpenseMembers(expenseId); //added
            }
            else
            {
                ViewBag.ErrorMessage = "No friends selected to add to the group.";
            }
            // return RedirectToAction("AddExpense", new { groupId = groupId });
            return RedirectToAction("ViewExpenseDetails", new { expenseId = expenseId });
        }

        //-----------------------individual expense before settleup-------------------
        public IActionResult ViewExpenseDetails(int expenseId) 
        {           
            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;
            List<ExpenseDetails> expenseDetails = _apiService.GetExpenseDetails(expenseId, userId);//4 e to E in 
            decimal overallOwed = 0;
            Dictionary<string, decimal> individualOwedAmounts = new Dictionary<string, decimal>();
            string paidByName = ""; // Name of the person who paid for the expense
            decimal paidAmount = 0; // Total amount paid by the person who paid for the expense
            string Description = "";
            decimal  totalAmount = 0;
            foreach (var expense in expenseDetails)
            {
                 Description = expense.ExpenseDescription; //added by me
                totalAmount=expense.TotalAmount;
                if (expense.PaidBy == expense.MemberId)
                {
                    // If the PaidBy is the same as the MemberId, this is the person who paid for the expense
                    paidByName = expense.Name;
                    paidAmount += expense.Amount;
                    continue;
                }

                decimal owedAmount = expense.Paid ? expense.Amount : -expense.Amount;

                overallOwed += owedAmount;

                if (individualOwedAmounts.ContainsKey(expense.Name))
                {
                    individualOwedAmounts[expense.Name] += owedAmount;
                }
                else
                {
                    individualOwedAmounts.Add(expense.Name, owedAmount);
                }

                // Calculate the total amount paid by the person who paid for the expense
            }
            ViewBag.Description = Description;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.OverallOwed = overallOwed;
            ViewBag.IndividualOwedAmounts = individualOwedAmounts;
            ViewBag.PaidByName = paidByName;
            ViewBag.PaidAmount = paidAmount;
            ViewBag.ExpenseId = expenseId;
            int groupId = HttpContext.Session.GetInt32("NewGroupId") ?? -1; 
            ViewBag.GroupId = groupId;
            return View("ViewExpenseDetails", expenseDetails);
        }

        //-----------------Payments--------------------------
        public IActionResult GetPendingPayments(int expenseId)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;
            HttpContext.Session.SetInt32("expenseId", expenseId);
            List<ExpenseDetails> expenseDetails = _apiService.GetExpenseDetails(expenseId, userId);
            string paidByName = "";
            foreach (var expense in expenseDetails)
            {
                if (expense.PaidBy == expense.MemberId)
                {
                    paidByName = expense.Name;

                    break;
                }
            }
            ViewBag.PaidByName = paidByName;
            return View("Payments", expenseDetails);
        }

        public IActionResult ShowPayment(int id)
        {
            int ExpenseId = id;
            int userId = HttpContext.Session.GetInt32("UserId") ?? -1;
            int expenseId = HttpContext.Session.GetInt32("expenseId") ?? -1;
            List<ExpenseDetails> expenseDetails = _apiService.GetExpenseDetails(expenseId, userId);
            string paidByName = "";
            foreach (var expense in expenseDetails)
            {
                if (expense.PaidBy == expense.MemberId)
                {
                    paidByName = expense.Name;
                    break;
                }
            }
            ViewBag.ExpenseId = ExpenseId; // Make sure ExpenseId is assigned to ViewBag
            ViewBag.PaidByName = paidByName;
            return View("SettlePayments", expenseDetails);
        }
        public IActionResult RecordPayment(int id, decimal amount)
        {
            RecordPayment model = new RecordPayment();
            model.ExpenseId = id;
            model.Amount = amount;
            _apiService.EditPaymentAmount(model);
            ViewBag.SuccessMessage = "Payment recorded successfully.";
            //return View();
            int? expenseId = HttpContext.Session.GetInt32("expenseId") ?? -1;
            return RedirectToAction("ViewExpenseDetails", new { expenseId = expenseId.Value });

        }

        //------------------------------------Get Expenses Of GroupInvolved------------------------------
        public IActionResult UserInvolvedGroups()
        {
            int UserId = HttpContext.Session.GetInt32("UserId") ?? -1;
            var groups = _apiService.GetUserInvolvedGroups(UserId);
            return View(groups);


        }
        //below  action is same as GetExpenseOfGroup
        public IActionResult GetExpenseOfGroup_Involved(int groupId) //all expenses of the group
        {
            int UserId = HttpContext.Session.GetInt32("UserId") ?? -1;
            ViewBag.GroupId = groupId; // Pass groupId to the view
            HttpContext.Session.SetInt32("NewGroupId", groupId);
            List<ExpenseDetails> expenseDetails = _apiService.GetExpenseOfGroup(groupId, UserId);


            var groupedExpenses = expenseDetails.GroupBy(e => e.ExpenseId);

            // List to hold grouped expense details
            List<ExpenseGroupViewModel> expenseGroups = new List<ExpenseGroupViewModel>();

            foreach (var group in groupedExpenses)
            {
                decimal overallOwed = 0;
                Dictionary<string, decimal> individualOwedAmounts = new Dictionary<string, decimal>();
                string paidByName = ""; // Name of the person who paid for the expense
                decimal paidAmount = 0; // Total amount paid by the person who paid for the expense
                string Description = "";
                decimal TotalAmount = 0;
                // Get the ExpenseId from the group
                int expenseId = group.Key;
                decimal TotalGroupAmount = 0;

                foreach (var expense in group)
                {
                    TotalGroupAmount += expense.TotalAmount;
                    Description = expense.ExpenseDescription;
                    TotalAmount = expense.TotalAmount;
                    if (expense.PaidBy == expense.MemberId)
                    {
                        // If the PaidBy is the same as the MemberId, this is the person who paid for the expense
                        paidByName = expense.Name;
                        paidAmount += expense.Amount;
                        continue;
                    }

                    decimal owedAmount = expense.Paid ? expense.Amount : -expense.Amount;

                    overallOwed += owedAmount;

                    if (individualOwedAmounts.ContainsKey(expense.Name))
                    {
                        individualOwedAmounts[expense.Name] += owedAmount;
                    }
                    else
                    {
                        individualOwedAmounts.Add(expense.Name, owedAmount);
                    }
                }
                ViewBag.TotalGroupAmountSum = TotalGroupAmount; //added

                // Create view model for the expense group
                var expenseGroup = new ExpenseGroupViewModel
                {
                    ExpenseId = expenseId, // Assign the ExpenseId obtained from the group
                    OverallOwed = overallOwed,
                    IndividualOwedAmounts = individualOwedAmounts,
                    PaidByName = paidByName,
                    PaidAmount = paidAmount,
                    GroupId = groupId,
                    Description = Description,
                    TotalAmount = TotalAmount,
                    //TotalGroupAmount=TotalGroupAmount,
                };

                expenseGroups.Add(expenseGroup);
            }

            return View("GetExpenseOfGroup_Involved", expenseGroups);


        }

        //---not using below action
        public IActionResult GetExpenseOfGroupInvolved(/*int groupId*/) //all expenses of the group
        {
            int UserId = HttpContext.Session.GetInt32("UserId") ?? -1;
           // ViewBag.GroupId = groupId; // Pass groupId to the view
           // HttpContext.Session.SetInt32("NewGroupId", groupId);
            List<ExpenseDetails> expenseDetails = _apiService.GetExpenseOfGroupInvolved(/*groupId,*/ UserId);


            var groupedExpenses = expenseDetails.GroupBy(e => e.ExpenseId);

            // List to hold grouped expense details
            List<ExpenseGroupViewModel> expenseGroups = new List<ExpenseGroupViewModel>();

            foreach (var group in groupedExpenses)
            {
                decimal overallOwed = 0;
                Dictionary<string, decimal> individualOwedAmounts = new Dictionary<string, decimal>();
                string paidByName = ""; // Name of the person who paid for the expense
                decimal paidAmount = 0; // Total amount paid by the person who paid for the expense
                string Description = "";
                decimal TotalAmount = 0;
                // Get the ExpenseId from the group
                int expenseId = group.Key;
                decimal TotalGroupAmount = 0;

                foreach (var expense in group)
                {
                    TotalGroupAmount += expense.TotalAmount;
                    Description = expense.ExpenseDescription;
                    TotalAmount = expense.TotalAmount;
                    if (expense.PaidBy == expense.MemberId)
                    {
                        // If the PaidBy is the same as the MemberId, this is the person who paid for the expense
                        paidByName = expense.Name;
                        paidAmount += expense.Amount;
                        continue;
                    }

                    decimal owedAmount = expense.Paid ? expense.Amount : -expense.Amount;

                    overallOwed += owedAmount;

                    if (individualOwedAmounts.ContainsKey(expense.Name))
                    {
                        individualOwedAmounts[expense.Name] += owedAmount;
                    }
                    else
                    {
                        individualOwedAmounts.Add(expense.Name, owedAmount);
                    }
                }
                ViewBag.TotalGroupAmountSum = TotalGroupAmount; //added

                // Create view model for the expense group
                var expenseGroup = new ExpenseGroupViewModel
                {
                    ExpenseId = expenseId, // Assign the ExpenseId obtained from the group
                    OverallOwed = overallOwed,
                    IndividualOwedAmounts = individualOwedAmounts,
                    PaidByName = paidByName,
                    PaidAmount = paidAmount,
                   // GroupId = groupId,
                    Description = Description,
                    TotalAmount = TotalAmount,
                    //TotalGroupAmount=TotalGroupAmount,
                };

                expenseGroups.Add(expenseGroup);
            }

            return View("GetExpenseOfGroupInvolved", expenseGroups);


        }
    }
}






