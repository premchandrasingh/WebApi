using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.ExceptionHandling;

namespace PPF.API
{
    public static class ApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Enabling Web Api attribute routing
            config.MapHttpAttributeRoutes();


            ErrorConfig.Configure(config);
            Formatter.SetJsonFormatter();
            Formatter.SetXmlFormatter();
        }
    }
}
