
using Newtonsoft.Json;
using System;
using System.Net.Http;
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
        //public  void AddGroup(AddNewGroup model)
        //{
        //    //string apiUrl = "http://localhost:5001/api/SplitWise/AddGroup";
        //    //var content=new StringContent(JsonConvert.SerializeObject(model));
        //    //var response= _httpClient.PostAsJsonAsync(apiUrl, content).Result;
        //    //response.EnsureSuccessStatusCode();

        //}
        public void AddGroup(AddNewGroup model)
        {
            string apiUrl = "http://localhost:5001/api/SplitWise/AddGroup";
            var response = _httpClient.PostAsJsonAsync(apiUrl, model).Result;
            response.EnsureSuccessStatusCode();
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

    }
}

