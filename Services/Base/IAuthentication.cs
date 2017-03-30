using System.Security.Claims;

namespace MyAdmin.Application.Services.Base
{
    public interface IAuthentication
    {
        ClaimsPrincipal Login(string username, string password, bool rememberMe);
    }
}