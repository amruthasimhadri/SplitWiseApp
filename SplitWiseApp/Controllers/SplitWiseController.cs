using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SplitWiseApp.Models;
using System.Reflection;

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
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel user)
        {
            int? response = _apiService.VerifyUser(user);

            if (response != null)
            {    
                int userId=(int)response.Value;
                HttpContext.Session.SetInt32("UserId", userId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password";
                return View();
            }


        }
        public IActionResult Dashboard()
        {
            int UserId = HttpContext.Session.GetInt32("UserId") ?? -1;
            return View();
        }

        
        public IActionResult CreateGroup()
        {
            var categories = _apiService.GetGroupTypes();
            ViewBag.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
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

            _apiService.AddGroup(newGroup);
            ViewBag.SuccessMessage = "Group created successfully!";

            var categories = _apiService.GetGroupTypes();
            ViewBag.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            return View(group);
        }


    }
}






