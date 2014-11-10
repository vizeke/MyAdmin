using MyAdmin.Mvc.Models;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System;
using Common.Logging;

namespace MyAdmin.Mvc.Servicos
{
    public class QueryManager
    {
        //private ILog log = LogManager.GetCurrentClassLogger();

        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Common.Logging.Log4Net.Log4NetLogger

        protected string _tipoBD;
        protected string _connString;
        protected string _appPath;

        protected UserModel _user;

        public QueryManager(UserModel user, string tipoDB, string connString, string appPath)
        {
            _user = user;
            _tipoBD = tipoDB;
            _connString = connString;
            _appPath = appPath;
        }

        public object ExecuteQuery(string query, string fileName = "", bool saveFile = false)
        {
            //SalvaLog
            log.Trace(this.getJsonExecutaQuery(query, fileName, saveFile));

            if (_tipoBD == SistemasModel.TipoServidorBD.SqlServer.ToString())
            {
                ExecuteSqlServer client = new ExecuteSqlServer(_connString, saveFile, fileName, _appPath);
                return client.ExecuteQuery(query);
            }
            else if (_tipoBD == SistemasModel.TipoServidorBD.Oracle.ToString())
            {
                ExecuteOracle client = new ExecuteOracle(_connString, saveFile, fileName, _appPath);
                return client.ExecuteQuery(query);
            }
            else
            {
                return new
                {
                    msg = "Database not supported"
                };
            }
        }

        public object GetStructureDB()
        {
            //SalvaLog
            log.Trace(this.getJsonStructureDB());

            if (_tipoBD == SistemasModel.TipoServidorBD.SqlServer.ToString())
            {
                ExecuteSqlServer client = new ExecuteSqlServer(_connString);
                return client.GetDBStructure();
            }
            else if (_tipoBD == SistemasModel.TipoServidorBD.Oracle.ToString())
            {
                ExecuteOracle client = new ExecuteOracle(_connString);
                return client.GetDBStructure();
            }
            else
            {
                return new
                {
                    msg = "Database not supported"
                };
            }
        }

        public string getJsonExecutaQuery(string query, string fileName, bool saveFile)
        {
            SqlConnectionStringBuilder builder = null;
            try { builder = new SqlConnectionStringBuilder(this._connString); }
            catch (FormatException) { }

            return JsonConvert.SerializeObject(new
            {
                method = "ExecuteQuery",
                user = this._user,
                query = query,
                dbUser = builder != null ? builder.UserID : "",
                catalog = builder != null ? builder.InitialCatalog : "",
                dataSource = builder != null ? builder.DataSource : "",
                saveFile = saveFile,
                fileName = fileName
            });
        }

        public string getJsonStructureDB()
        {
            SqlConnectionStringBuilder builder = null;
            try { builder = new SqlConnectionStringBuilder(this._connString); }
            catch (FormatException) { }

            return JsonConvert.SerializeObject(new
            {
                method = "GetStructureDB",
                user = this._user,
                dbUser = builder != null ? builder.UserID : "",
                catalog = builder != null ? builder.InitialCatalog : "",
                dataSource = builder != null ? builder.DataSource : ""
            });
        }
    }
}