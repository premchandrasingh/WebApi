using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using PPF.API.Models;
using PPF.API.Providers;
using PPF.API.Results;
using PPF.API.Services;
using PPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace PPF.API.ApiControllers
{
    /// <summary>
    /// Externals Login providers
    /// </summary>
    [RoutePrefix("api/ExternalLogins")]
    public class ExternalLoginsController : ApiControllerBase
    {
        public ExternalLoginsController(IGate gate) : base(gate)
        {

        }

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("Providers", Name = "Providers")]
        [ResponseType(typeof(List<ExternalLoginViewModel>))]
        public IHttpActionResult GetExternalLoginProviders(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = StartupBearer.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return Ok(logins);
        }



        // ignore bearer tokens, and it can be accessed if there is external cookie set by external authority (Facebook or Google)
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }
            var userManagerService = Gate.UserModule.UserManagerService;

            Member user = (await userManagerService.FindAsync(new ExternalUserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey))).Data;

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                Op<ClaimsIdentity> oAuthIdentityResult = await userManagerService.GenerateUserIdentityAsync(user, OAuthDefaults.AuthenticationType);
                Op<ClaimsIdentity> cookiesIdentityResult = await userManagerService.GenerateUserIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);
                ClaimsIdentity oAuthIdentity = oAuthIdentityResult.Data;
                ClaimsIdentity cookiesIdentity = cookiesIdentityResult.Data;

                AuthenticationProperties properties = OAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookiesIdentity);
            }
            else
            {
                var newExLogin = new ExternalLogin { Provider = externalLogin.LoginProvider, ProviderKey = externalLogin.ProviderKey };
                var newMem = new Member()
                {
                    UserName = externalLogin.UserName,
                    FirstName = externalLogin.FirstName,
                    LastName = externalLogin.LastName,
                    Email = externalLogin.UserName,
                    IsExternal = true
                };

                await userManagerService.CreateExternalLoginAsync(newExLogin, newMem);

                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        //private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        //{

        //    Uri redirectUri;

        //    var redirectUriString = GetQueryString(Request, "redirect_uri");

        //    if (string.IsNullOrWhiteSpace(redirectUriString))
        //    {
        //        return "redirect_uri is required";
        //    }

        //    bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

        //    if (!validUri)
        //    {
        //        return "redirect_uri is invalid";
        //    }

        //    var clientId = GetQueryString(Request, "client_id");

        //    if (string.IsNullOrWhiteSpace(clientId))
        //    {
        //        return "client_Id is required";
        //    }

        //    var client = _repo.FindClient(clientId);

        //    if (client == null)
        //    {
        //        return string.Format("Client_id '{0}' is not registered in the system.", clientId);
        //    }

        //    if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
        //    {
        //        return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
        //    }

        //    redirectUriOutput = redirectUri.AbsoluteUri;

        //    return string.Empty;

        //}

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }


        private class ExternalLoginData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    FirstName = identity.FindFirstValue(ClaimTypes.GivenName),
                    LastName = identity.FindFirstValue(ClaimTypes.Surname),
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Email),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }
    }

}