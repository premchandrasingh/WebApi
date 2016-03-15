using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using PPF.API.Models;
using PPF.API.Services.User;
using PPF.Models;
using Microsoft.Owin;
using PPF.API.Services;

namespace PPF.API.Providers
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public OAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }
        /// <summary>
        /// The ValidateClientAuthentication method is the place where you’ll make calls to a database or membership 
        /// system to determine if a user is providing the correct credentials. If so, the method will add data into
        /// the OWIN request context for GrantResourceOwnerCredentials to pick up and place into claims 
        /// (which ultimately become part of the auth ticket)
        /// 
        /// context.Validated(); method will do above task for you
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManagerService = context.OwinContext.GetUserManagerV2();

            Op<Member> user = await userManagerService.FindAsync(context.UserName, context.Password);
            

            if (user.Data == null)
            {
                context.SetError("invalid_grant", user.Meta.Message);
                return;
            }

            Op<ClaimsIdentity> oAuthIdentityResult = await userManagerService.GenerateUserIdentityAsync(user.Data, OAuthDefaults.AuthenticationType);
            Op<ClaimsIdentity> cookiesIdentityResult = await userManagerService.GenerateUserIdentityAsync(user.Data, CookieAuthenticationDefaults.AuthenticationType);
            ClaimsIdentity oAuthIdentity = oAuthIdentityResult.Data;
            ClaimsIdentity cookiesIdentity = cookiesIdentityResult.Data;

            AuthenticationProperties properties = CreateProperties(user.Data.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }
        
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
                

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
                

    }


    public static class OwinExtensions
    {
        public static IUserManagerService GetUserManagerV2(this IOwinContext context)
        {
            var gate = context.Get<ServiceFactory>().GetService<IGate>();
            var userManagerService = gate.UserModule.UserManagerService;
            return userManagerService;
        }

    }

}