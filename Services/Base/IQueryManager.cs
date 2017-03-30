namespace MyAdmin.Application.Services.Base
{
    public interface IQueryManager
    {
        object ExecuteQuery(string tipoDB, string connString, string appPath, string query, string fileName = "", bool saveFile = false);
        object GetStructureDB(string tipoDB, string connString, string appPath);
    }
}
