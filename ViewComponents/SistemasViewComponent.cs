using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyAdmin.Application.Models;

namespace MyAdmin.ViewComponents
{
    public class SistemasViewComponent : ViewComponent
    {
        private readonly IConfigurationRoot configuration;

        public SistemasViewComponent(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        public IViewComponentResult Invoke(int numberOfItems)
        {
            var model = new SistemasModel();
            model.AllowConnStringDefinition = bool.Parse(configuration["AllowConnStringDefinition"]);
            return View(model);
        }
    }
}