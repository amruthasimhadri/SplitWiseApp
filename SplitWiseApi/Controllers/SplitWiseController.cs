using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
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
        //public bool IsUserAlreadyRegistered(string email)
        //{
        //    SqlCommand cmd = new SqlCommand("CheckUserExistence", con);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@Email", email);

        //    int count = (int)cmd.ExecuteScalar();
        //    return count > 0;
        //}

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

            //cmd.ExecuteNonQuery();

            int groupId = Convert.ToInt32(outputParam.Value);

           // return Ok(new { GroupId = groupId });
           return Ok(groupId);

        }





        [HttpGet]
        [Route("GetGroupTypes")]
        public IEnumerable<GroupTypeModel> GetGroupTypes()
        {
            List<GroupTypeModel> Types = new List<GroupTypeModel>();
            SqlCommand cmd = new SqlCommand("Select Id,Name from Types", con);
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                Types.Add(new GroupTypeModel
                {
                    Id = r.GetInt32("Id"),
                    Name = r.GetString("Name"),
                });

            }
            return Types;
        }
    }
}


