using System.Security.Claims;
using Newtonsoft.Json;

namespace MyAdmin.Application.Models
{
    public class UserModel : ClaimsPrincipal
    {
        public string name { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}