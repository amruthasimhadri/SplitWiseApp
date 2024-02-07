using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

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
    }
}
