namespace MyAdmin.Application.Services.Base
{
    public interface IExecuteQuery
    {
        object ExecuteQuery(string query);
        object GetDBStructure();
    }
}
