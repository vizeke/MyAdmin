/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyAdmin.Application.Models;
using MyAdmin.Application.Services.Base;
using Newtonsoft.Json;
*/
namespace MyAdmin.Application.Infrastructure
{
    //IInterceptionBehavior
    public class AuditoriaInterceptionBehavior 
    {
/*        private AuditoriaModel model;

        private readonly IAuditoria auditoria;

        public AuditoriaInterceptionBehavior(IAuditoria _auditoria)
        {
            auditoria = _auditoria;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            FormsIdentity principal = HttpContext.Current.User as FormsIdentity;
            model = new AuditoriaModel()
            {
                user = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(System.IdentityModel.Claims.ClaimTypes.Name).Value,
                userId = int.Parse(System.Security.Claims.ClaimsPrincipal.Current.FindFirst(System.IdentityModel.Claims.ClaimTypes.Sid).Value),
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(),
                url = HttpContext.Current.Request.Url.ToString(),
                className = input.Target.ToString(),
                methodName = input.MethodBase.Name,
                jsonParameters = "",
                jsonObject = "",
                obs = ""
            };

            //Process parameters list
            List<object> listParameters = new List<object>();
            for (int i = 0; i < input.Arguments.Count; i++)
            {
                var builder = getConnectionStringBuilder(input.Arguments[i].ToString());
                if (builder != null)
                {
                    builder.Remove("password");
                    listParameters.Add(builder.ToString());
                }
                else
                {
                    listParameters.Add(input.Arguments[i]);
                }
            }
            model.jsonParameters = JsonConvert.SerializeObject(listParameters);

            // Invoke the next behavior in the chain.
            var result = getNext()(input, getNext);

            //Search values to audito post execution
            var msgValue = result.ReturnValue.GetType().GetProperty("msg").GetValue(result.ReturnValue);
            if (msgValue != null)
            {
                model.obs = msgValue.ToString();
            }

            //Save Audit
            auditoria.Auditar(model);

            //End Interception
            return result;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        private SqlConnectionStringBuilder getConnectionStringBuilder(string value)
        {
            try
            {
                return new SqlConnectionStringBuilder(value);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }*/
    }
}
