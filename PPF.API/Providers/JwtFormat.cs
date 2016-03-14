using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Thinktecture.IdentityModel.Tokens;

namespace PPF.API.Providers
{

    /// <summary>
    /// Application Json Web Token formatter
    /// </summary>
    public class JwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private const string AudiencePropertyKey = "userName";

        private readonly string _issuer = string.Empty;

        public JwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            
            string audienceId = data.Properties.Dictionary.ContainsKey(AudiencePropertyKey) ? data.Properties.Dictionary[AudiencePropertyKey] : null;

            if (string.IsNullOrWhiteSpace(audienceId))
                throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");

            var userManager = HttpContext.Current.Request.GetOwinContext().GetUserManagerV2();
            var user = userManager.FindByNameAsync(audienceId).Result.Data;


            string symmetricKeyAsBase64 = user.Password;

            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

            //using (HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes("Secret key")))
            //{
            //    calculatedSignature = ByteToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(requestToken)));
            //}

            HmacSigningCredentials signingKey = new HmacSigningCredentials(keyByteArray);

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            JwtSecurityToken token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;
        }

        private string ByteToString(IEnumerable<byte> data)
        {
            return string.Concat(data.Select(b => b.ToString("x2")));
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}