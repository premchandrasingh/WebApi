using PPF.API.Areas.HelpPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace PPF.API
{
    public class MvcRouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // ENABLING ATTRIBUTE ROUTING IN MVC (CONFIGURATION FOR WEB API IS DIFFERENT). THIS LINE SHOULD BE BEFORE "AreaRegistration.RegisterAllAreas()";
            //THIS LINE SHOULD BE BEFORE "AreaRegistration.RegisterAllAreas()" METHOD IF ANY;
            routes.MapMvcAttributeRoutes();

            // AreaRegistration.RegisterAllAreas();
            
#if DEBUG
            HelpPageConfig.Register(GlobalConfiguration.Configuration);
#endif


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );//.DataTokens = new RouteValueDictionary(new { area = "helppage" });
        }
    }
}
