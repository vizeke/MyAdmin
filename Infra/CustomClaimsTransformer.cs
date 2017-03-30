/*using System;
using System.Collections.Generic;
using System.Security.Claims;
using MyAdmin.Application.Services.Base;*/

namespace MyAdmin.Application.Infra
{
    // ClaimsAuthenticationManager
    public class CustomClaimsTransformer 
    
    {
/*        private readonly IAuthentication authentication;

        public CustomClaimsTransformer() { }

        public CustomClaimsTransformer(IAuthentication _authentication)
        {
            authentication = _authentication;
        }

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (!incomingPrincipal.Identity.IsAuthenticated)
            {
                return base.Authenticate(resourceName, incomingPrincipal);
            }

            //TODO: check where to put claims transformation logic, it's in the Autenticação service.
            //ClaimsPrincipal transformedPrincipal = DressUpPrincipal(incomingPrincipal.Identity.Name); 
            ClaimsPrincipal transformedPrincipal = incomingPrincipal;

            CreateSession(transformedPrincipal);

            return transformedPrincipal;
        }

        private void CreateSession(ClaimsPrincipal transformedPrincipal)
        {
            SessionSecurityToken sessionSecurityToken = new SessionSecurityToken(transformedPrincipal, TimeSpan.FromHours(8));
        }

        void SessionAuthenticationModule_SessionSecurityTokenReceived(object sender, SessionSecurityTokenReceivedEventArgs e)
        {
            SessionAuthenticationModule sam = sender as SessionAuthenticationModule;
            //e.SessionToken = sam.CreateSessionSecurityToken();

            CreateSession(e.SessionToken.ClaimsPrincipal);

            e.ReissueCookie = true;
        }

        void SessionAuthenticationModule_SessionSecurityTokenCreated(object sender, SessionSecurityTokenCreatedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private ClaimsPrincipal DressUpPrincipal(String userName)
        {
            List<Claim> claims = new List<Claim>();

            //simulate database lookup
            if (userName.IndexOf("andras", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                claims.Add(new Claim(ClaimTypes.Country, "Sweden"));
                claims.Add(new Claim(ClaimTypes.GivenName, "Andras"));
                claims.Add(new Claim(ClaimTypes.Name, "Andras"));
                claims.Add(new Claim(ClaimTypes.Role, "IT"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.GivenName, userName));
                claims.Add(new Claim(ClaimTypes.Name, userName));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom"));
        }*/
    }
}
