using Microsoft.AspNetCore.Mvc;

namespace MyAdmin.Mvc.Controllers
{
    public class ConfigurationController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

    }
}
