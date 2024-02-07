using Microsoft.AspNetCore.Mvc;

namespace SplitWiseApp.Controllers
{
    public class SplitWiseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
