using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace SplitWiseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SplitWiseController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly SqlConnection con;
        public SplitWiseController(IConfiguration configuration)
        {
            _configuration = configuration;
            con = new SqlConnection(_configuration.GetConnectionString("connectionstring"));
            con.Open();
        }
        [HttpPost]
        [Route("AddUser")]
        public void AddUser([FromBody] UserModel model)
        {
            SqlCommand cmd = new SqlCommand("AddUser", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Password", model.Password);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@PhNo", model.Number);
            cmd.ExecuteNonQuery();

        }
 
        [HttpPost]
        [Route("VerifyUser")]

        public IActionResult VerifyUser([FromBody] LoginModel user)
        {
            SqlCommand cmd = new SqlCommand("Login", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return Unauthorized();
            }

        }

        //----------------Add Group---------------------
        [HttpPost]
        [Route("AddGroup")]
        public IActionResult AddGroup([FromBody] AddNewGroup model)
        {

            SqlCommand cmd = new SqlCommand("AddNewGroup", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TypeId", model.TypeId);
            cmd.Parameters.AddWithValue("@GroupName", model.GroupName);
            cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);

            SqlParameter outputParam = new SqlParameter();
            outputParam.ParameterName = "@GroupId";
            outputParam.SqlDbType = SqlDbType.Int;
            outputParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(outputParam);
            cmd.ExecuteNonQuery();

            int groupId = Convert.ToInt32(outputParam.Value);

            return Ok(groupId);

        }

        [HttpGet]
        [Route("GetGroupTypes")]
        public IEnumerable<GroupTypeModel> GetGroupTypes()
        {
            List<GroupTypeModel> Types = new List<GroupTypeModel>();
            SqlCommand cmd = new SqlCommand("Select Id,Name,ImageUrl from Types", con);
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                Types.Add(new GroupTypeModel
                {
                    Id = r.GetInt32("Id"),

                    Name = r.GetString("Name"),
                    ImageUrl=r.GetString("ImageUrl"),
                });

            }
            return Types;
        }
        [HttpGet]
        [Route("GetAllGroups")]
        public IEnumerable<GroupTypeModel> GetAllGroups(int userId)
        {
            List<GroupTypeModel> groups = new List<GroupTypeModel>();
            SqlCommand cmd = new SqlCommand("[GetAllGroups]", con);
            cmd.CommandType= CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CreatedBy", userId);
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                groups.Add(new GroupTypeModel
                {
                    Id = r.GetInt32("Id"),
                    Name = r.GetString("GroupName"),
                    ImageUrl = r.GetString("ImageUrl"),
                    GroupType=r.GetString("Name")
                });

            }
            return groups;
        }
        //---------------------------
        [HttpGet]
        [Route("GetExpenseOfGroup")]
        public IEnumerable<ExpenseDetails> GetExpenseOfGroup(int groupId, int userId)
        {

            List<ExpenseDetails> expenseDetails = new List<ExpenseDetails>();
            SqlCommand cmd = new SqlCommand("GetExpenseDetailsOfGroup", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@GroupId", groupId);
            cmd.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                expenseDetails.Add(new ExpenseDetails
                {
                    Id = (int)rdr["Id"],
                    Name = rdr["Name"].ToString(),
                    Amount = (decimal)rdr["Amount"],
                    Paid = (bool)rdr["Paid"],
                    GroupName = (string)rdr["GroupName"],
                    PaidBy = (int)rdr["PaidBy"],
                    MemberId = (int)rdr["MemberId"],
                  ExpenseId = (int)rdr["ExpenseId"],
                    ExpenseDescription = (string)rdr["Description"],
                    TotalAmount = (decimal)rdr["TotalAmount"],

                });
            }
            return expenseDetails;
        }

        //-------------------------------

        [HttpGet]
        [Route("GetFriends")]
        public IEnumerable<Friends> GetFriends(int userId)
        {
            List<Friends> friends = new List<Friends>();
            SqlCommand cmd = new SqlCommand("GetFriends", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                friends.Add(new Friends
                {
                    FriendId = (int)rdr["FriendId"],
                    FriendName = rdr["Name"].ToString()

                });
            }
            return friends;
        }

        [HttpPost]
        [Route("AddGroupMembers")]
        public IActionResult AddGroupMember([FromBody] GroupMembers groupMember)
        {

            SqlCommand cmd = new SqlCommand("INSERT INTO GroupMembers (GroupId, FriendId) VALUES (@GroupId, @FriendId)", con);
            cmd.Parameters.AddWithValue("@GroupId", groupMember.GroupId);
            cmd.Parameters.AddWithValue("@FriendId", groupMember.FriendId);
            cmd.ExecuteNonQuery();
            return Ok();

        }
        //-------------Select Expense Membbers---------------
        [HttpGet]
        [Route("SelectExpenseMembers")]
        public IEnumerable<Friends> SelectExpenseMembers(int userId,int groupId)
        {
            List<Friends> friends = new List<Friends>();
            SqlCommand cmd = new SqlCommand("GetFriendForExpense", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@GroupId", groupId);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                friends.Add(new Friends
                {
                    FriendId = (int)rdr["FriendId"],
                    FriendName = rdr["Name"].ToString()

                });
            }
            return friends;
        }

        [HttpPost]
        [Route("AddExpenseMembers")]
        public IActionResult AddExpenseMembers([FromBody] ExpenseMembers expenseMember)
        {

            SqlCommand cmd = new SqlCommand("INSERT INTO SelectExpenseMembers (ExpenseId, FriendId) VALUES (@ExpenseId, @FriendId)", con);
            cmd.Parameters.AddWithValue("@ExpenseId", expenseMember.ExpenseId);
            cmd.Parameters.AddWithValue("@FriendId", expenseMember.FriendId);
            cmd.ExecuteNonQuery();
            return Ok();

        }





        //---------------------------------------------------
        [HttpPost]
        [Route("AddFriend")]
        public IActionResult AddFriend([FromBody] Friends model)
        {
            int result = GetFriendIdFromDb(model.FriendEmail, model.FriendName);
            if (result != -1)
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Friends VALUES (@UserId, @FriendId)", con);
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@FriendId", result);

                cmd.ExecuteNonQuery();

                return Ok(result);
            }
            else
            {
                return Unauthorized();

            }

             int GetFriendIdFromDb(string email, string friendName)
            {
                SqlCommand cmd = new SqlCommand("Select Id from Users where Name=@Name and Email=@Email", con);
                cmd.Parameters.AddWithValue("@Name", friendName);
                cmd.Parameters.AddWithValue("Email", email);
                var a = cmd.ExecuteScalar();
                if (a != null && a != DBNull.Value)
                {
                    return Convert.ToInt32(a);
                }
                else
                {
                    return -1;
                }
            }
        }

        [HttpGet]
        [Route("GetExpenseModel")]
        public ActionResult<IEnumerable<AddExpense>> GetExpenseModel(int groupId, int userId)
        {
            AddExpense model = GetModel(groupId, userId);
            return Json(model);
        }
        private AddExpense GetModel(int groupId, int userId)
        {
            SqlCommand cmd = new SqlCommand("GetGroupMembers", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@groupId", groupId);
            cmd.Parameters.AddWithValue("@userId", userId);
            SqlDataReader dr = cmd.ExecuteReader();
            AddExpense model = new AddExpense();
            while (dr.Read())
            {
                model.groupMembers.Add(new SelectListItem { Text = dr.GetString(1), Value = dr.GetInt32(0).ToString() });

            }
            dr.Close();
            return model;
        }
        [HttpPost]
        [Route("AddExpense")]
        public IActionResult AddExpense([FromBody] AddExpenseToDB expense)
        {
            SqlCommand cmd = new SqlCommand("AddExpense", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@GroupId", expense.GroupId);
            cmd.Parameters.AddWithValue("@Description", expense.Description);
            cmd.Parameters.AddWithValue("@Amount", expense.Amount);
            cmd.Parameters.AddWithValue("@PaidBy", expense.PaidBy);

            SqlParameter outputParam = new SqlParameter();
            outputParam.ParameterName = "@ExpenseId";
            outputParam.SqlDbType = SqlDbType.Int;
            outputParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(outputParam);
            cmd.ExecuteNonQuery();



            int expenseId = Convert.ToInt32(outputParam.Value);


            return Ok(expenseId);
        }

        
        [HttpGet]
        [Route("AddExpenseMembers")]
        public ActionResult AddExpenseMembers(int expenseId) //Insert to Table
        {
            SqlCommand cmd = new SqlCommand("AddExpenseMembers_Test", con); // changed sp name
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@expenseId", expenseId);
            cmd.ExecuteNonQuery();
            return Ok();
        }


        [HttpGet]
        [Route("GetExpenseDetails")]
        public IEnumerable<ExpenseDetails> GetExpenseDetails(int expenseId, int userId)
        {
            List<ExpenseDetails> expenseDetails = new List<ExpenseDetails>();
            SqlCommand cmd = new SqlCommand("GetExpenseDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
            cmd.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                expenseDetails.Add(new ExpenseDetails
                {
                    Id = (int)rdr["Id"],
                    Name = rdr["Name"].ToString(),
                    Amount = (decimal)rdr["Amount"],
                    Paid = (bool)rdr["Paid"],
                    GroupName = (string)rdr["GroupName"],
                    PaidBy = (int)rdr["PaidBy"],
                    MemberId = (int)rdr["MemberId"],
                    ExpenseDescription = (string)rdr["Description"],
                    TotalAmount = (decimal)rdr["TotaAmount"]


                });
            }
            return expenseDetails;
        }

        [HttpPost]
        [Route("EditPaymentAmount")]
        public void EditPaymentAmount([FromBody] RecordPayment model)
        {
            SqlCommand cmd = new SqlCommand("EditPaymentAmount", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", model.ExpenseId);
            cmd.Parameters.AddWithValue("@Amount", model.Amount);           
            cmd.ExecuteNonQuery();
        }


        //------------------------ Get Expense Details of Groups Involved
        [HttpGet]
        [Route("GetUserInvolvedGroups")]
        public IEnumerable<GroupTypeModel> GetUserInvolvedGroups(int userId)
        {
            List<GroupTypeModel> groups = new List<GroupTypeModel>();
            SqlCommand cmd = new SqlCommand("[GetUserInvolvedGroups_Test]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                groups.Add(new GroupTypeModel
                {
                    Id = r.GetInt32("Id"),
                    Name = r.GetString("GroupName"),
                    ImageUrl = r.GetString("ImageUrl"),
                    GroupType = r.GetString("TypeName")
                });

            }
            return groups;
        }

        [HttpGet]
        [Route("GetExpenseOfGroupInvolved")]
        public IEnumerable<ExpenseDetails> GetExpenseOfGroupInvolved(int groupId, int userId)
        {

            List<ExpenseDetails> expenseDetails = new List<ExpenseDetails>();
            SqlCommand cmd = new SqlCommand("[GetExpenseDetailsOfUser_InvolvedGroups]", con);
            cmd.CommandType = CommandType.StoredProcedure;
           // cmd.Parameters.AddWithValue("@GroupId", groupId);
            cmd.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                expenseDetails.Add(new ExpenseDetails
                {
                    Id = (int)rdr["Id"],
                    Name = rdr["Name"].ToString(),
                    Amount = (decimal)rdr["Amount"],
                    Paid = (bool)rdr["Paid"],
                    GroupName = (string)rdr["GroupName"],
                    PaidBy = (int)rdr["PaidBy"],
                    MemberId = (int)rdr["MemberId"],
                    ExpenseId = (int)rdr["ExpenseId"],
                    ExpenseDescription = (string)rdr["Description"],
                    TotalAmount = (decimal)rdr["TotalAmount"],

                });
            }
            return expenseDetails;
        }

    }
}



