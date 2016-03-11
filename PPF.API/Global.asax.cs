using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

#if DEBUG
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif

namespace PPF.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            UnityConfig.RegisterComponents();

            GlobalConfiguration.Configure(ApiConfig.Register);
            ApiFilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            MvcFilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            MvcRouteConfig.RegisterRoutes(RouteTable.Routes);
            MvcBinderConfig.Register(GlobalConfiguration.Configuration);
            MvcAntiForgeryConfig.Configure(GlobalConfiguration.Configuration);
            MvcBundleConfig.RegisterBundles(BundleTable.Bundles);
#if DEBUG
            SSLValidator.OverrideSSLValidation();
#endif

        }
    }

#if DEBUG
    /// <summary>
    /// This will always validate if target api or site has invalid SSL certificate. 
    /// </summary>
    public static class SSLValidator
    {
        private static bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static void OverrideSSLValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback = OnValidateCertificate;
            ServicePointManager.Expect100Continue = true;

            //// ======= OR =================
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };
        }
    }
#endif
}
