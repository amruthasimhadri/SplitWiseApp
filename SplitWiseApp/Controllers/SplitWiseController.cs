using Microsoft.AspNetCore.Mvc;
using SplitWiseApp.Models;

namespace SplitWiseApp.Controllers
{
    public class SplitWiseController : Controller
    {
        private readonly ApiService _apiService;
        public SplitWiseController(ApiService apiService)
        {
            _apiService = apiService;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            _apiService.AddUser(user);
            ViewBag.SuccessMessage = "Registered Successfully";
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
