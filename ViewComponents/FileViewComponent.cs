using Microsoft.AspNetCore.Mvc;
using MyAdmin.Application.Models;

namespace MyAdmin.ViewComponents
{
    public class FileViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new FileModel();
            return View(model);
        }
    }
}