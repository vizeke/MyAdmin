using System;
using System.Collections.Generic;
using System.IO;
// using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Application.Models;
using MyAdmin.Application.Services;

namespace MyAdmin.Mvc.Controllers
{
    // [Authorize]
    public class QueryController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private IConfigurationRoot configuration { get; set; }
        public readonly QueryManager queryManager;
        public QueryController(IHostingEnvironment hostingEnvironment, IConfigurationRoot configuration)
        {
            this.queryManager = new QueryManager();
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            QueryModel model = new QueryModel()
            {
                Temas = BuscaTemas()
            };
            return View(model);
        }

        public IActionResult SalvarArquivo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ExecutaQuery([FromBody]QueryExecutaQueryDTO dto)
        {
            var data = queryManager.ExecuteQuery(dto.Banco, dto.ConnectionString, dto.Query, dto.SalvaArquivo ?? false, dto.NomeArquivo, hostingEnvironment.WebRootPath);
            return Ok(data);
        }

        [HttpPost]
        public IActionResult GetStructureDB([FromBody]QueryStructureDBDTO dto)
        {
            var data = queryManager.GetDBStructure(dto.Banco, dto.ConnectionString);
            return Ok(data);
        }

        /* Private methods */
        private List<string> BuscaTemas()
        {
            List<string> themes = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(hostingEnvironment.WebRootPath + "\\codemirror\\theme\\");

            foreach (FileInfo file in dir.GetFiles())
            {
                themes.Add(Path.GetFileNameWithoutExtension(file.Name));
            }

            return themes;
        }
    }

    public class QueryStructureDBDTO
    {
        public string Sistema { get; set; }
        public string Banco { get; set; }
        public string ConnectionString { get; set; }
    }

    public class QueryExecutaQueryDTO
    {
        public string Sistema { get; set; }
        public string Ambiente { get; set; }
        public string Banco { get; set; }
        public string Query { get; set; }
        public string ConnectionString { get; set; }
        public bool? SalvaArquivo { get; set; }
        public String NomeArquivo { get; set; }
    }
}
