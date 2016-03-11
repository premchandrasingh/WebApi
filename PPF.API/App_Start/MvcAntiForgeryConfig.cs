using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PPF.API
{
    public class MvcAntiForgeryConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            //https://stack247.wordpress.com/2013/02/22/antiforgerytoken-a-claim-of-type-nameidentifier-or-identityprovider-was-not-present-on-provided-claimsidentity/

            /*
             At default, ASP.NET MVC uses User.Identity.Name as anti-forgery token to validate form submitted. Worth to note that by default, ASP.NET MVC is not Claims-aware app.

             When converting to Claims-aware app, ASP.NET MVC doesn’t use User.Identity.Name as the anti-forgery token anymore. Instead, it attempts to use the NameIdentifier and IdentityProvider ClaimType. 
             */
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;

            //List<Claim> _claims = new List<Claim>();
            //_claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", _user.Email));
            //_claims.Add(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", _user.Email));

        }
    }
}