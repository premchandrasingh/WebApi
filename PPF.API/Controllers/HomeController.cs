using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPF.API.Controllers
{
    //[RouteArea("external")]
    [Route("{action}")]
    [RoutePrefix("Home")]
    public class HomeController : Controller
    {
        [Route]
        [Route("~/", Name = "defaultAttributeRoute")] // overriding route prefix
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
