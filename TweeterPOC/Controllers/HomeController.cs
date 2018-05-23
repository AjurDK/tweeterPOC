using Microsoft.AspNetCore.Mvc;

namespace TweeterPOC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
