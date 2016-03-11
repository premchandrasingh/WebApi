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

namespace PPF.API.Results
{
    public class ErrorResult : IHttpActionResult
    {
        ExceptionHandlerContext _errorContext;
        public ErrorResult(ExceptionHandlerContext context)
        {
            _errorContext = context;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            dynamic error = new ExpandoObject();
            error.Message = "Server encountered an internal error";
            error.Code = 500;
#if DEBUG
            error.Message = _errorContext.Exception.Message;
            //error.IsAuthenticated = _errorContext.Request.GetRequestContext().Principal.Identity.IsAuthenticated;
            //error.ErrorType = _errorContext.Exception.GetType();
            error.Source = _errorContext.Exception.Source;
            error.StackTrace = _errorContext.Exception.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Take(8).ToList();
#endif

            var response = new HttpResponseMessage()
            {
                Content = new ObjectContent(typeof(object), error, Formatter.JsonFormatter),
                RequestMessage = _errorContext.Request,
                StatusCode = HttpStatusCode.InternalServerError
            };


            return Task.FromResult(response);
        }
    }
}