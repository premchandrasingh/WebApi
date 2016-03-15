using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PPF.API.StartupBearer))]
//[assembly: OwinStartup(typeof(PPF.API.StartupJwt))]
namespace PPF.API
{
    public partial class StartupBearer
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }

    public partial class StartupJwt
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
