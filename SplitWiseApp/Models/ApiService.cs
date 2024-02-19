
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    }
}

