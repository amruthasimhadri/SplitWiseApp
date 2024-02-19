using Microsoft.AspNetCore.Http;
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

            //_apiService.AddGroup(newGroup);
            //ViewBag.SuccessMessage = "Group created successfully!";

            //var categories = _apiService.GetGroupTypes();
            //ViewBag.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            //return View(group);
            int? groupId = _apiService.AddGroup(newGroup);

            if (groupId != null)
            {
                HttpContext.Session.SetInt32("GroupId", groupId.Value);
                ViewBag.SuccessMessage = "Group created successfully!";
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to create group.";
            }

            var categories = _apiService.GetGroupTypes();
            ViewBag.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            return View(group);

        }

        public IActionResult GetFriends()
        {
            int userId=HttpContext.Session.GetInt32("UserId") ??-1;
            var friends = _apiService.GetFriends(userId);
            return View(friends);
        }
        

        [HttpPost]
        public IActionResult AddGroupMembers( [FromForm] List<int> friends)
        {
            int groupId = HttpContext.Session.GetInt32("GroupId") ?? -1;
            if (friends != null && friends.Any())
            {
                foreach (var friendId in friends)
                {
                    GroupMembers groupMember = new GroupMembers
                    {
                        GroupId = groupId,
                        FriendId = friendId
                    };
                    _apiService.AddGroupMember(groupMember);
                }
                ViewBag.SuccessMessage = "Group members added successfully!";
            }
            else
            {
                ViewBag.ErrorMessage = "No friends selected to add to the group.";
            }
            return RedirectToAction("Friends");
        }



    }
}






