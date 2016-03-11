using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Dynamic;
using PPF.API.Helper;

namespace PPF.API.Filters.Api
{
    /// <summary>
    /// This attribute will verify the model/argument(s) has a valid state and will optionally assure model/argument(s) is/are not null.
    /// By default it assure model is not null
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private readonly Func<Dictionary<string, object>, bool> _validate = null;
        private readonly bool _checkForNullArgument = false;
        /// <summary>
        /// By default verifying the model/argument(s) is/are not null is set to TRUE
        /// </summary>
        public ValidateModelAttribute()
            : this(true)
        { }
        /// <summary>
        /// Turn off checking model/argument(s) is not null 
        /// </summary>
        /// <param name="checkNullArguments">Passing 'false' will turn off checking model/argument(s) is/are not null. 
        /// Warning: This can throw null reference exception when accessing your model/argument </param>
        public ValidateModelAttribute(bool checkNullArguments)
            : this(arguments => arguments.ContainsValue(null))
        {
            _checkForNullArgument = checkNullArguments;
        }

        /// <summary>
        /// Custom function to check model/argument(s) is/are not null
        /// </summary>
        /// <param name="checkCondition">Custom function to check method parameter are not null</param>
        public ValidateModelAttribute(Func<Dictionary<string, object>, bool> checkCondition)
        {

            _validate = checkCondition;
        }

        /// <summary>
        /// Whenever model is Invalid assign Response value and stop execution right there
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            // *** possible candiate status for model validation are PreconditionFailed, ExpectationFailed

            dynamic output = new ExpandoObject();

            // Checking the passing argument is null or not
            if (_checkForNullArgument && _validate(actionContext.ActionArguments))
            {
                output.Message = "Argument can't be null";
                actionContext.Response = CreateResponse(actionContext, output);
            }
            else
            {
                var modelState = actionContext.ModelState;
                var param = actionContext.ActionArguments.Count > 0 ? actionContext.ActionArguments[actionContext.ActionArguments.Keys.First()] : null;
                if (!modelState.IsValid)
                {

                    var validationErrors = (from state in modelState
                                            from error in state.Value.Errors
                                            select new Error
                                            {
                                                Property = ConvertCase.To(state.Key.Remove(0, state.Key.IndexOf('.') + 1), ConvertCase.Case.CamelCase, new[] { '.' }),
                                                Message = error.ErrorMessage
                                            }).ToList();

                    #region Only to resolve duplicate key which results in exception
                    Int16 tracker = 0;
                    validationErrors = validationErrors.OrderBy(er => er.Property).ToList();
                    validationErrors = validationErrors.Select((err, index) =>
                    {
                        if (index > 0 && (validationErrors[index - 1].Property == err.Property || validationErrors[index - 1].Property == err.Property + "_" + tracker))
                        {
                            tracker++;
                            err.Property = err.Property + "_" + tracker;
                        }
                        else if (index > 0 && (validationErrors[index - 1].Property != err.Property || validationErrors[index - 1].Property != err.Property + "_" + tracker))
                        {
                            tracker = 0;
                        }
                        return err;
                    }).ToList();
                    #endregion

                    output.ValidationErrors = new Dictionary<string, string>();

                    foreach (var err in validationErrors)
                    {
                        output.ValidationErrors.Add(err.Property, err.Message);
                    }

                    //actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, modelState);
                    actionContext.Response = CreateResponse(actionContext, output);
                }
            }
        }


        protected virtual HttpResponseMessage CreateResponse(HttpActionContext actionContext, object data)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new ObjectContent(typeof(object), data, Formatter.JsonFormatter),
                ReasonPhrase = "Invalid input data",
                RequestMessage = actionContext.Request,
            };

            return resp;
        }

        private class Error
        {
            public string Property { get; set; }
            public string Message { get; set; }
        }
    }
}