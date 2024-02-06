using Microsoft.AspNetCore.Mvc;
using SplitWiseApp.Models;
using System.Diagnostics;

namespace SplitWiseApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index1234()
        {
            return View();
        }
        public IActionResult Greeshma()
        {
            return View();
        }

        public IActionResult Amrutha1()
        {
            return View();
        }

        public IActionResult New1234()

        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}