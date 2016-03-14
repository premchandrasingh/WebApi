using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PPF.API.Providers
{
    /// <summary>
    /// Refresh token provider
    /// Good article: http://bitoftech.net/2014/07/16/enable-oauth-refresh-tokens-angularjs-app-using-asp-net-web-api-2-owin/
    /// Good explaination: http://stackoverflow.com/questions/20637674/owin-security-how-to-implement-oauth2-refresh-tokens
    /// </summary>
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString("n");

            //copy properties and set the desired lifetime of refresh token
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(5) //SET DATETIME to 5 Minutes
            };

            /*CREATE A NEW TICKET WITH EXPIRATION TIME OF 5 MINUTES 
             *INCLUDING THE VALUES OF THE CONTEXT TICKET: SO ALL WE 
             *DO HERE IS TO ADD THE PROPERTIES IssuedUtc and 
             *ExpiredUtc to the TICKET*/
            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);


            #region Saving refresh token to DB
            //var clientId = Convert.ToInt64(context.Ticket.Properties.Dictionary["userId"]);
            //var factory = context.OwinContext.Get<ServiceFactory>();
            //var svc = factory.GetService<IGate>();

            //var token = new RefreshToken()
            //{
            //    Id = Hashing.Compute(guid),
            //    ClientId = clientId,
            //    Subject = context.Ticket.Identity.Name,
            //    IssuedUtc = context.Ticket.Properties.IssuedUtc,
            //    ExpiresUtc = refreshTokenProperties.ExpiresUtc,
            //    prProtectedTicket = context.SerializeTicket()
            //};

            //svc.UserModule.UserManagerService.AddRefreshToken(token); 
            #endregion




            // maybe only create a handle the first time, then re-use
            _refreshTokens.TryAdd(guid, refreshTokenTicket);


            // consider storing only the hash of the handle
            context.SetToken(guid);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {

            #region Matching refresh token from DB and removing old one
            //var factory = context.OwinContext.Get<ServiceFactory>();
            //var svc = factory.GetService<IGate>();
            //string hashedToken = Hashing.Compute(context.Token);

            //var refreshToken = svc.UserModule.UserManagerService.FindRefreshToken(hashedToken);
            //if (refreshToken != null)
            //{
            //    //Get protectedTicket from refreshToken class
            //    context.DeserializeTicket(refreshToken.ProtectedTicket);
            //    var result = svc.UserModule.UserManagerService.RemoveRefreshToken(hashedToken);
            //}

            #endregion


            AuthenticationTicket ticket;
            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
            }


        }
    }
}