using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.OAuth;
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
    /// http://odetocode.com/blogs/scott/archive/2015/01/15/using-json-web-tokens-with-katana-and-webapi.aspx
    /// </summary>
    public class JwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _audience = string.Empty;

        private readonly string _issuer = string.Empty;

        private readonly int _expiryTimeInMinutes =0;

        private readonly string _key = string.Empty;

        public JwtFormat(string key, string issuer, string audience, int expiryTimeInMinutes)
        {
            _key = key;
            _issuer = issuer;
            _audience = audience;
            _expiryTimeInMinutes = expiryTimeInMinutes;

        }
        
        public string SignatureAlgorithm {  get { return "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256"; } }

        public string DigestAlgorithm  { get { return "http://www.w3.org/2001/04/xmlenc#sha256"; } }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null) throw new ArgumentNullException("data");
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_expiryTimeInMinutes);
            var keyArr = Microsoft.Owin.Security.DataHandler.Encoder.TextEncodings.Base64Url.Decode(_key);

            //var certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2();
            //var cert = new X509SecurityKey(certificate);
            

            var signingCredentials = new SigningCredentials(
                                        new InMemorySymmetricSecurityKey(keyArr), // DO not use InMemorySymmetricSecurityKey for Production. Use X509SecurityKey class
                                        SignatureAlgorithm,
                                        DigestAlgorithm);
            var token = new JwtSecurityToken(_issuer, _audience, data.Identity.Claims, now, expires, signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // OAuth server never uses the Unprotect method
        // Unprotect seems like it would be useful when verifying  a token, but that’s just not how these components work
        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}