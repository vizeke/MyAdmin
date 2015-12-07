using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAdmin.Mvc.Servicos.Base
{
    public interface IExecuteQuery
    {
        object ExecuteQuery(string pQuery);

        object GetDBStructure();
    }
}
