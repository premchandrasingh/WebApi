using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PPF.API.Filters.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateAntiForgeryTokenGracefullyAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public ValidateAntiForgeryTokenGracefullyAttribute()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redirectUrl"></param>
        public ValidateAntiForgeryTokenGracefullyAttribute(string redirectUrl)
        {
            this.RedirectUrl = redirectUrl;
        }

        /// <summary>
        /// Redirect url where you want to redirect when antiforgery token error encountered
        /// </summary>
        public string RedirectUrl { get; set; }


        private void ValidateRequestHeader(HttpRequestBase request)
        {
            string cookieToken = String.Empty;
            string formToken = String.Empty;
            string tokenValue = request.Headers["RequestVerificationToken"];
            if (!String.IsNullOrEmpty(tokenValue))
            {
                string[] tokens = tokenValue.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }
            System.Web.Helpers.AntiForgery.Validate(cookieToken, formToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                //if (filterContext.RequestContext.HttpContext.Request.RequestType == "POST")
                //{
                //}

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    ValidateRequestHeader(filterContext.HttpContext.Request);
                }
                else
                {
                    System.Web.Helpers.AntiForgery.Validate();
                }
            }
            catch (HttpAntiForgeryException e)
            {
                filterContext.Controller.TempData["Error"] = "Something went wrong with your request.";
                if (!string.IsNullOrEmpty(RedirectUrl))
                {
                    filterContext.Result = new RedirectResult(RedirectUrl);
                }
                else
                {
                    if (filterContext.HttpContext.Request.RawUrl.ToLower().Contains("logout"))
                        filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.RawUrl.ToLower().Replace("logout", "login"));

                    filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.RawUrl);
                }
            }
        }
    }

}