using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using PPF.API.Providers;
using PPF.API.Models;
using PPF.API.Services.User;
using Microsoft.Owin.Security.Jwt;

namespace PPF.API
{
    public partial class StartupBearer
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {

            app.CreatePerOwinContext<ServiceFactory>(ServiceFactory.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });

            //// Use a cookie to temporarily store information about a user logging in with a third party login provider
            //// THIS SHOULD BE ENABLE IF YOUR APPLICAION HAS EXTERNAL(THIRD-PARTY) AUTHENTICATION
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "PPF_API";

            var expiry = 30;

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new OAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/ExternalLogins/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(expiry),
                AllowInsecureHttp = true,
                RefreshTokenProvider = new RefreshTokenProvider(),
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
            

            StartupExternalAuth.Configure(app);

        }
    }


    public partial class StartupJwt
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static JwtBearerAuthenticationOptions JWtOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {

            app.CreatePerOwinContext<ServiceFactory>(ServiceFactory.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "PPF_API";

            var issuer = "http://localhost:58714";
            var audience = "all";
            var secretKey = "UHxNtYMRYwvfpO1dS5pWLKL0M2DgOj40EbN4SoBWgfc"; //qMCdFDQuF23RV1Y-1Gq9L3cF3VmuFwVbam4fMTdAfpo
            var expiry = 30;

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new OAuthProvider(PublicClientId),
                //AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(expiry),
                AllowInsecureHttp = true,
                RefreshTokenProvider = new RefreshTokenProvider(),
                AccessTokenFormat = new Providers.JwtFormat(secretKey, issuer, audience, expiry)
            };

            // Enable the application to use bearer tokens to authenticate users
            //app.UseOAuthBearerTokens(OAuthOptionsV2);

            //var keyArray = Convert.FromBase64String(secretKey);
            var arr = Microsoft.Owin.Security.DataHandler.Encoder.TextEncodings.Base64Url.Decode(secretKey);

            JWtOptions = new JwtBearerAuthenticationOptions()
            {
                AllowedAudiences = new[] { audience },
                IssuerSecurityTokenProviders = new[] { new SymmetricKeyIssuerSecurityTokenProvider(issuer, arr) }
            };

            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseJwtBearerAuthentication(JWtOptions);



            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }

}
