using System.Web;
using System.Web.Mvc;

namespace PPF.API
{
    public class MvcFilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            //// MAKE SURE MVC PAGES ARE HTTPS
            //filters.Add(new PPF.API.Filters.Mvc.RequireHttpsAttribute());


            //// ENABLING AUTHORIZATION TO ALL MVC PAGES
            //filters.Add(new System.Web.Mvc.AuthorizeAttribute());
        }
    }
}
