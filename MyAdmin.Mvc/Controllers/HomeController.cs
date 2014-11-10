using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using MyAdmin.Mvc.Models;
using MyAdmin.Mvc.Servicos;
using System.Linq;
using System.IO;
using System.DirectoryServices;
using System.Text;

namespace MyAdmin.Mvc.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            GetGroups("LDAP://dcsrvprdc06.datacenter.es.gov.br.local");

            AuthenticateUser("PRODEST\vinicius.barbosa", "abCD1234", "LDAP://dcsrvprdc06.datacenter.es.gov.br.local");

            return View();
        }

        private bool AuthenticateUser(string username, string password, string LdapPath)
        {
            DirectoryEntry entry = new DirectoryEntry(LdapPath, username, password);
            try
            {
                // Bind to the native AdsObject to force authentication.
                Object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if (null == result)
                {
                    return false;
                }
                // Update the new path to the user in the directory
                LdapPath = result.Path;
                string _filterAttribute = (String)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception("Error authenticating user." + ex.Message);
            }
            return true;
        }

        public string GetGroups(string _path)
        {
            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(cn=vinicius.barbosa)";
            search.PropertiesToLoad.Add("memberOf");
            StringBuilder groupNames = new StringBuilder();

            try
            {
                SearchResult result = search.FindOne();
                int propertyCount = result.Properties["memberOf"].Count;
                string dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    dn = (string)result.Properties["memberOf"][propertyCounter];
                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }
                    groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                    groupNames.Append("|");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }
            return groupNames.ToString();
        }

        [HttpGet]
        public ActionResult Index()
        {
            QueryModel model = new QueryModel();

            model.Temas = BuscaTemas();

            return View(model);
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Sistemas()
        {
            return PartialView("_Sistemas", new SistemasModel());
        }

        [HttpGet]
        public ActionResult File()
        {
            return PartialView("_File");
        }

        [HttpPost]
        public JsonResult ExecutaQuery(string Banco, string Query, string ConnectionString, bool SalvaArquivo = false, string NomeArquivo = "")
        {
            try
            {
                var ip = Request.ServerVariables["REMOTE_ADDR"].ToString();
                var name = HttpContext.User.Identity.Name;
                MyAdmin.Mvc.Servicos.QueryManager serv = new QueryManager(this.GetUserModel(), Banco, ConnectionString, Request.PhysicalApplicationPath);

                var data = serv.ExecuteQuery(Query, NomeArquivo, SalvaArquivo);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SalvarArquivo()
        {
            return View();
        }

        private List<string> BuscaTemas()
        {
            List<string> themes = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Content\\codemirror\\theme\\");

            foreach (FileInfo file in dir.GetFiles())
            {
                themes.Add(Path.GetFileNameWithoutExtension(file.Name));
            }

            return themes;
        }

        public JsonResult GetStructureDB(string Banco, string ConnectionString)
        {
            try
            {

                MyAdmin.Mvc.Servicos.QueryManager serv = new QueryManager(this.GetUserModel(), Banco, ConnectionString, Request.PhysicalApplicationPath);

                var data = serv.GetStructureDB();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetConnectionsObject()
        {
            var connString = new List<object>();
            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
            {
                if (!string.IsNullOrEmpty(item.ConnectionString))
                {
                    var info = item.Name.Split('-');
                    connString.Add(new { sistema = info[0], ambiente = info[1], connString = item.ConnectionString });
                }
            }

            return Json(connString, JsonRequestBehavior.AllowGet);
        }

        private UserModel GetUserModel()
        {
            return new UserModel() { ip = Request.ServerVariables["REMOTE_ADDR"].ToString(), name = HttpContext.User.Identity.Name };
        }
    }
}
