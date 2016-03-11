using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace PPF.API.Filters.Api
{
    /// <summary>
    /// This filter force request to be of https
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                actionContext.Response = CreateResponse(actionContext,
                    new
                    {
                        Status = HttpStatusCode.Forbidden,
                        Code = 40443,
                        Message = "HTTPS connection required",
                        DeveloperMessage = "This api supports only https connection. Also check your client certificate",
                        MoreInfo = "http://api.xyz.com/developer/docs/errors#40443"
                    });
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }

        /// <summary>
        /// Create Response
        /// </summary>
        /// <param name="actionContext">Action context</param>
        /// <param name="data">data of response</param>
        /// <returns></returns>
        protected virtual HttpResponseMessage CreateResponse(HttpActionContext actionContext, object data)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new ObjectContent(typeof(object), data, Formatter.JsonFormatter),
                ReasonPhrase = "HTTPS connection required",
                RequestMessage = actionContext.Request
            };

            return resp;
        }

    }
}