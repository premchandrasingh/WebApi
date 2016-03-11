using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPF.API.Filters.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireHttpsAttribute : System.Web.Mvc.RequireHttpsAttribute
    {
        public RequireHttpsAttribute()
            : this(permanent: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireHttpsAttribute"/> class.
        /// </summary>
        /// <param name="permanent">Whether the redirect to HTTPS should be a permanent redirect.</param>
        public RequireHttpsAttribute(bool permanent)
        {
            this.Permanent = permanent;
        }

        /// <summary>
        /// Gets a value indicating whether the redirect to HTTPS should be a permanent redirect.
        /// </summary>
        public bool Permanent { get; private set; }

        protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
        {
            // only redirect for GET requests, otherwise the browser might not propagate the verb and request 
            // body correctly.
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You must use HTTPS connection");
            }

            // redirect to HTTPS version of page
            string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
#if DEBUG
            url = "https://" + filterContext.HttpContext.Request.Url.Host + ":44301" + filterContext.HttpContext.Request.RawUrl;
#endif

            filterContext.Result = new RedirectResult(url, this.Permanent);
        }
    }
}