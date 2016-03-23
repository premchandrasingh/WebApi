using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace PPF.API
{
    public class StartupExternalAuth
    {
        public static GoogleOAuth2AuthenticationOptions GoogleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }

        public static void Configure(IAppBuilder app)
        {
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //Configure Facebook External Login
            FacebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = "1584683298488791",
                AppSecret = "d97309ba55dc08357b3dce06cd2ee68c",
                Provider = new FacebookAuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {
                        context.Identity.AddClaim(new Claim("your-claim", "your-value"));
                        return Task.FromResult(0);
                    }
                }
            };

            app.UseFacebookAuthentication(FacebookAuthOptions);

            GoogleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "486569447131-m2u2ctjvh1d889b44snm7ir2fjr5ev0q.apps.googleusercontent.com",
                ClientSecret = "aj5wKQbSeS6x06fO5TkXlMkr",
                Provider = new GoogleOAuth2AuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {
                        
                        context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
                        context.Identity.AddClaim(new Claim("your-claim", "your-value"));
                        return Task.FromResult(0);
                    }
                },
                //// "signin-google" is default where the user-agent will be returned. The middleware will process this request when it arrives
                ////this is never called by MVC, but needs to be registered at your oAuth provider
                //// This CallbackPath MUST be different from the ChallengeResult in Account/ExternalLogin
                CallbackPath = new PathString("/signin-google")
            };
            GoogleAuthOptions.Scope.Add("email");
            app.UseGoogleAuthentication(GoogleAuthOptions);
        }
    }
    
}