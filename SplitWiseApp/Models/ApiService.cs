
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

            //if (response.IsSuccessStatusCode)
            //{
            //    try
            //    {
            //        var content = response.Content.ReadAsStringAsync().Result;

            //        // Log or print out the response content for debugging
            //        Console.WriteLine("Response Content: " + content);

            //        // Parse the content as JSON
            //        var jsonObject = JObject.Parse(content);

            //        // Extract the groupId value
            //        if (jsonObject.TryGetValue("groupId", out JToken groupIdToken))
            //        {
            //            if (int.TryParse(groupIdToken.ToString(), out int groupId))
            //            {
            //                return groupId;
            //            }
            //            else
            //            {
            //                // Log or print out if parsing groupId fails
            //                Console.WriteLine("Failed to parse groupId: " + groupIdToken.ToString());
            //            }
            //        }
            //        else
            //        {
            //            // Log or print out if groupId key is not found
            //            Console.WriteLine("groupId key not found in response.");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        // Log or handle other exceptions
            //        Console.WriteLine("Error: " + ex.Message);
            //        return null;
            //    }
            //}
            //else
            //{
            //    // Log or handle non-successful response
            //    Console.WriteLine("Failed with status code: " + response.StatusCode);
            //}

            //return null;
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

