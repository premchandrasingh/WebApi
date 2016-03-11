using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace PPF.API
{
    public class ErrorConfig
    {
        /// <summary>
        /// Configure Api Exception configuration
        /// </summary>
        /// <param name="config"></param>
        public static void Configure(HttpConfiguration config)
        {
            config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLogger());
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
        }
    }
}