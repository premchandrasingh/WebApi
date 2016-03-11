using PPF.API.Results;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace PPF.API
{
    /// <summary>
    /// Since we can have ONLY ONE exception handler. This exection handler will handle all expections
    /// </summary>
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new ErrorResult(context);
        }

        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            if (!ShouldHandle(context))
            {
                return Task.FromResult(0);
            }
            else
            {
                context.Result = new ErrorResult(context);
                return Task.FromResult(context);
            }
        }

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }

        
    }



}