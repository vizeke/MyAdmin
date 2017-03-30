using Microsoft.AspNetCore.Mvc;

namespace MyAdmin.Mvc.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            //return RedirectToAction("Login", "Account");
            return RedirectToAction("Index","Query");
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
    }
}
