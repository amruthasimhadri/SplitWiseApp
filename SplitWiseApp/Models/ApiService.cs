
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SplitWiseApp.Models
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void AddUser(UserModel model)
        {
            string apiUrl = "http://localhost:5001/api/SplitWise/AddUser";
            var response = _httpClient.PostAsJsonAsync(apiUrl, model).Result;
        }
        public int? VerifyUser(LoginModel model)
        {
            string apiUrl = "http://localhost:5001/api/SplitWise/VerifyUser";

            var response = _httpClient.PostAsJsonAsync(apiUrl, model).Result;
            if (response.IsSuccessStatusCode)
            {
                var userIdString = response.Content.ReadAsStringAsync().Result;
                if (int.TryParse(userIdString, out int userId))
                {
                    return userId;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        
        public int? AddGroup(AddNewGroup model)
        {
            string apiUrl = "http://localhost:5001/api/SplitWise/AddGroup";
            var response = _httpClient.PostAsJsonAsync(apiUrl, model).Result;
            if (response.IsSuccessStatusCode)
            {
                var groupId = response.Content.ReadAsStringAsync().Result;
                if (int.TryParse(groupId, out int GroupId))
                {
                    return GroupId;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
  
        }

        public List<GroupTypeModel> GetGroupTypes()
        {
            string apiUrl = "http://localhost:5001/api/SplitWise/GetGroupTypes";
            var response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();
            var jsonString=response.Content.ReadAsStringAsync().Result;
            var types = JsonConvert.DeserializeObject<List<GroupTypeModel>>(jsonString);
            return types;
        }

        public List<GroupTypeModel> GetAllGroups(int UserId)
        {
            string apiUrl = $"http://localhost:5001/api/SplitWise/GetAllGroups?userId={UserId}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var groups = JsonConvert.DeserializeObject<List<GroupTypeModel>>(jsonString);
            return groups;

        }
        //Expenses of all groups

        public List<ExpenseDetails> GetExpenseOfGroup(int groupId, int userId)
        {
            string apiUrl = $"http://localhost:5001/api/SplitWise/GetExpenseOfGroup?groupId={groupId}&userId={userId}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var expenseDetails = JsonConvert.DeserializeObject<List<ExpenseDetails>>(jsonString);
            return expenseDetails;
        }



        //-----------------------
        public List<Friends> GetFriendsToAddInGroup(int userId,int groupId)
        {
            string apiUrl = $"http://localhost:5001/api/SplitWise/GetFriendsToAddInGroup?userId={userId}&groupId={groupId}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var friends = JsonConvert.DeserializeObject<List<Friends>>(jsonString);
            return friends;
        }
        public List<Friends> GetFriends(int userId)
        {
             string apiUrl=$"http://localhost:5001/api/SplitWise/GetFriends?userId={userId}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var friends = JsonConvert.DeserializeObject<List<Friends>>(jsonString);
            return friends;
        }

        public void AddGroupMember(GroupMembers groupMember)
        {
            string apiUrl = "http://localhost:5001/api/SplitWise/AddGroupMembers";
            var response = _httpClient.PostAsJsonAsync(apiUrl, groupMember).Result;
            response.EnsureSuccessStatusCode();
        }
        //-----------------------Add Expense Members---------------------------
        public List<Friends> GetExpenseFriends(int groupId,int userId)
        {
            string apiUrl = $"http://localhost:5001/api/SplitWise/SelectExpenseMembers?groupId={groupId}&userId={userId}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var friends = JsonConvert.DeserializeObject<List<Friends>>(jsonString);
            return friends;
        }

        public void AddExpenseMembers(ExpenseMembers expenseMember)
        {
            string apiUrl = "http://localhost:5001/api/SplitWise/AddExpenseMembers";
            var response = _httpClient.PostAsJsonAsync(apiUrl, expenseMember).Result;
            response.EnsureSuccessStatusCode();
        }
        //------------------------------------------------------
        public int? AddFriend(Friends friend)
        {
            string apiUrl = "http://localhost:5001/api/SplitWise/AddFriend";
            var response = _httpClient.PostAsJsonAsync(apiUrl, friend).Result;
            if (response.IsSuccessStatusCode)
            {
                var friendIdString = response.Content.ReadAsStringAsync().Result;
                if (int.TryParse(friendIdString, out int friendId))
                {
                    return friendId;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }


      
        public AddExpense GetExpenseModel(int groupId, int userId)

        {

            string apiUrl = $"http://localhost:5001/api/SplitWise/GetExpenseModel?groupId={groupId}&userId={userId}";

            var response = _httpClient.GetAsync(apiUrl).Result;

            response.EnsureSuccessStatusCode();

            var jsonString = response.Content.ReadAsStringAsync().Result;

            var expenseModel = JsonConvert.DeserializeObject<AddExpense>(jsonString);

            return expenseModel;

        }

        public int? AddExpense(AddExpenseToDB expense)

        {

            string apiUrl = "http://localhost:5001/api/SplitWise/AddExpense";

            var response = _httpClient.PostAsJsonAsync(apiUrl, expense).Result;

            if (response.IsSuccessStatusCode)

            {

                var expenseIdString = response.Content.ReadAsStringAsync().Result;

                if (int.TryParse(expenseIdString, out int expenseId))

                {

                    return expenseId;

                }

                else

                {

                    return null;

                }

            }

            else

            {

                return null;

            }

        }

        public void AddExpenseMembers(int expenseId)

        {

            string apiUrl = $"http://localhost:5001/api/SplitWise/AddExpenseMembers?expenseId={expenseId}";

            var response = _httpClient.GetAsync(apiUrl).Result;

            response.EnsureSuccessStatusCode();

        }


        public List<ExpenseDetails> GetExpenseDetails(int expenseId, int userId)
        {
            string apiUrl = $"http://localhost:5001/api/SplitWise/GetExpenseDetails?expenseId={expenseId}&userId={userId}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var expenseDetails = JsonConvert.DeserializeObject<List<ExpenseDetails>>(jsonString);
            return expenseDetails;
        }

        public void EditPaymentAmount(RecordPayment model)
        {
            string apiUrl = "http://localhost:5001/api/SplitWise/EditPaymentAmount";
            var response = _httpClient.PostAsJsonAsync(apiUrl, model).Result;
        }

        //Expenses of Group Involved

        public List<GroupTypeModel> GetUserInvolvedGroups(int UserId)
        {
            string apiUrl = $"http://localhost:5001/api/SplitWise/GetUserInvolvedGroups?userId={UserId}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var groups = JsonConvert.DeserializeObject<List<GroupTypeModel>>(jsonString);
            return groups;

        }
        public List<ExpenseDetails> GetExpenseOfGroupInvolved(/*int groupId,*/ int userId)
        {
            string apiUrl = $"http://localhost:5001/api/SplitWise/GetExpenseOfGroupInvolved?userId={userId}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var expenseDetails = JsonConvert.DeserializeObject<List<ExpenseDetails>>(jsonString);
            return expenseDetails;
        }

    }
}


