using MyAdmin.Application.Models;
using MyAdmin.Application.Services.Base;

namespace MyAdmin.Application.Services
{
    public class QueryManager : IQueryManager
    {
        // private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public object ExecuteQuery(string tipoDB, string connString, string appPath, string query, string fileName = "", bool saveFile = false)
        {
            if (tipoDB == SistemasModel.TipoServidorBD.SqlServer.ToString())
            {
                ExecuteSqlServer client = new ExecuteSqlServer(connString, saveFile, fileName, appPath);
                return client.ExecuteQuery(query);
            }
            else if (tipoDB == SistemasModel.TipoServidorBD.Oracle.ToString())
            {
                ExecuteOracle client = new ExecuteOracle(connString, saveFile, fileName, appPath);
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

        public object GetStructureDB(string tipoDB, string connString, string appPath)
        {
            if (tipoDB == SistemasModel.TipoServidorBD.SqlServer.ToString())
            {
                ExecuteSqlServer client = new ExecuteSqlServer(connString);
                return client.GetDBStructure();
            }
            else if (tipoDB == SistemasModel.TipoServidorBD.Oracle.ToString())
            {
                ExecuteOracle client = new ExecuteOracle(connString);
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
    }
}