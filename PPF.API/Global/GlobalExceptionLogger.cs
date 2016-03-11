using Microsoft.Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using Microsoft.AspNet.Identity.Owin;
using System.Globalization;
using System.Security.Claims;
using System.Dynamic;

namespace PPF.API
{
    /// <summary>
    /// Api generic exception logger
    /// </summary>
    public class GlobalExceptionLogger : ExceptionLogger
    {
      

      
        public override void Log(ExceptionLoggerContext context)
        {
            // Log your error here
        }

        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            // Log your error here
            return Task.FromResult(0);
        }

        public override bool ShouldLog(ExceptionLoggerContext context)
        {
            return true;
        }
        
    }
        
    public static class LogCompiler
    {
        public static object GetLog(IOwinContext owinContext, Exception exeption)
        {
            if (exeption == null)
                exeption = new Exception("Generic error");

            string userName = null;
            if (owinContext.Authentication.User != null && owinContext.Authentication.User.Identity != null)
                userName = owinContext.Authentication.User.Identity.Name;

            var httpContext = owinContext.Get<HttpContextBase>("System.Web.HttpContextBase").ApplicationInstance.Context;

            var innerExceptions = new List<string>();
            Exception tempExp = exeption.InnerException;
            while (tempExp != null)
            {
                innerExceptions.Add(tempExp.Message);
                tempExp = tempExp.InnerException;
            }

            dynamic log = new ExpandoObject();
            log.Title = exeption.Message;
            log.InnerExceptions = innerExceptions.Count > 0 ? string.Join(Environment.NewLine, innerExceptions) : null;
            log.StackTrace = exeption == null ? null : exeption.StackTrace;
            log.SourceApplication = "API";
            log.ClientIP = owinContext.Request.RemoteIpAddress;
            log.UserName = userName;
            log.Scheme = owinContext.Request.Scheme;
            log.Verb = owinContext.Request.Method;
            log.Url = Convert.ToString(owinContext.Request.Uri);
            log.Browser = string.Format("{0} {1}", httpContext.Request.Browser.Browser, httpContext.Request.Browser.Version);
            log.UrlReferrer = httpContext.Request.UrlReferrer != null ? httpContext.Request.UrlReferrer.ToString() : null;
            log.Culture = CultureInfo.CurrentCulture.Name;
            log.LogTimeUtc = DateTime.UtcNow;

            return log;
        }

        public static object GetLog(Exception exception)
        {
            dynamic log = new ExpandoObject();
            if (HttpContext.Current == null || HttpContext.Current.Request == null)
                return log;

            var owinContext = HttpContext.Current.Request.GetOwinContext();
            return GetLog(owinContext, exception);
        }
    }
         
}