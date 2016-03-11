using System.Web.Http;
using System.Web.Mvc;

namespace PPF.API.Areas.HelpPage
{
    //// THIS CLASS IS NO MORE REQUIRED AS MVC ATTRIBUTE ROUTING IS ENABLED. SEE RouteConfig.cs

    //public class HelpPageAreaRegistration : AreaRegistration
    //{
    //    public override string AreaName
    //    {
    //        get
    //        {
    //            return "HelpPage";
    //        }
    //    }

    //    public override void RegisterArea(AreaRegistrationContext context)
    //    {
    //        context.MapRoute(
    //            "HelpPage_Default",
    //            "Help/{action}/{apiId}",
    //            new { controller = "Help", action = "Index", apiId = UrlParameter.Optional });

    //        HelpPageConfig.Register(GlobalConfiguration.Configuration);
    //    }
    //}
}