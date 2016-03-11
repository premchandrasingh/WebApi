using PPF.API.Filters.Api;
using PPF.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PPF.API
{

    /// <summary>
    /// Base api controller
    /// </summary>
    //[RequireHttps]
    public class ApiControllerBase : ApiController
    {
        protected IGate Gate { get; set; }
        public ApiControllerBase(IGate gate)
        {
            Gate = gate;
        }
    }
}