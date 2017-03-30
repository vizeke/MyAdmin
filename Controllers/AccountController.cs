using Microsoft.AspNetCore.Mvc;
using MyAdmin.Application.Models;

namespace MyAdmin.Mvc.Controllers
{
    public class AccountController : Controller
    {
        // private readonly IAuthentication svcAuthentication;

        public AccountController(/*IAuthentication _svcAuthentication*/)
        {
            // svcAuthentication = _svcAuthentication;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authenticate(AuthenticationModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Query");
                /*var claimsPrincipal = svcAuthentication.Login(model.Name, model.Password, model.RememberMe);

                if (claimsPrincipal != null)
                {
                    ClaimsAuthenticationManager authManager = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager;
                    authManager.Authenticate(string.Empty, claimsPrincipal);
                    return RedirectToAction("Index", "Query");
                }*/
            }

            return View("Login", model);
        }

        [HttpPost]
        public IActionResult SignOut()
        {
            //Session.Remove("ClaimsPrincipal");
            return RedirectToAction("Login", "Account");
        }
    }
}
