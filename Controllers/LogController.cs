﻿using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Application.Models;

namespace MyAdmin.Mvc.Controllers
{
    public class LogController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public LogController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            DirectoryInfo dir = new DirectoryInfo(_hostingEnvironment.WebRootPath + "App_Data\\log\\");

            var model = new LogIndexModel()
            {
                Files = dir.GetFiles()
            };

            return View(model);
        }
    }
}