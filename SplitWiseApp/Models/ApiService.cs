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
            string apiUrl = "https://localhost:7056/api/SplitWiseApi/AddUser";


            var response = _httpClient.PostAsJsonAsync(apiUrl, model).Result;

        }
    }
}
