using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAdmin.Mvc.Models;

namespace MyAdmin.Mvc.Controllers
{
    public class LogController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            DirectoryInfo dir = new DirectoryInfo(Request.PhysicalApplicationPath + "App_Data\\log\\");

            var model = new LogIndexModel()
            {
                Files = dir.GetFiles()
            };

            return View(model);
        }
    }
}