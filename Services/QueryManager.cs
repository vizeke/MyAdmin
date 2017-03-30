using System;
using MyAdmin.Application.Services.Base;

namespace MyAdmin.Application.Services
{
    public class QueryManager
    {
        // private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public object ExecuteQuery(string typeDB, string connString, string query, bool saveFile = false, string fileName = "", string appPath = "")
        {
            IExecuteQuery client = this.GetDBClient(typeDB, connString, fileName, saveFile, appPath);
            return client.ExecuteQuery(query);
        }

        public object GetDBStructure(string typeDB, string connString)
        {
            IExecuteQuery client = this.GetDBClient(typeDB, connString);
            return client.GetDBStructure();
        }

        private IExecuteQuery GetDBClient(string typeDB, string connString, string fileName = "", bool saveFile = false, string appPath = "")
        {
            switch (typeDB)
            {
                case "SqlServer":
                    return new ExecuteSqlServer(connString, saveFile, fileName, appPath);
                case "Oracle":
                    return new ExecuteOracle(connString, saveFile, fileName, appPath);
                default:
                    throw new Exception("Database not supported");
            }
        }
    }
}