using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using PPF.API.Models;
using PPF.API.Providers;
using PPF.API.Results;
using PPF.API.Services.User;
using PPF.Models;
using PPF.API.Filters.Api;
using PPF.API.Services;

namespace PPF.API.Controllers
{
    /// <summary>
    /// Accounts
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts")]
    public class AccountController : ApiControllerBase
    {
        public AccountController(IGate gate) : base(gate)
        {

        }

        private string MakesureTableExist(string tableName)
        {
            var path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            path = path.Replace("file:///", "");
            var directory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), "Data");
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            var file = System.IO.Path.Combine(directory, string.Format("{0}.table.json", tableName));
            if (!System.IO.File.Exists(file))
                System.IO.File.Create(file);

            return path;
        }

        [AllowAnonymous]
        [Route("Test", Name = "Test")]
        public IHttpActionResult Test()
        {
            var s = MakesureTableExist("dfdf");

            return Ok(s);
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("RegisterV2")]
        [ValidateModel]
        public async Task<IHttpActionResult> RegisterV2(RegisterBindingModel model)
        {
            //// THIS CHECK IS NO MORE REQUIRED IF YOU APPLY CUSTOM "ValidateModel" ATTRIBUTE
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var user = new Member() { UserName = model.Email, Email = model.Email, Password = model.Password, IsExternal = false };


            var result = await Gate.UserModule.UserManagerService.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Meta.Message);
            }

            return Ok(result.Data);
        }



    }
}
