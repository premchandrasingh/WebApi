using Microsoft.Practices.Unity;
using PPF.API.Services;
using PPF.API.Services.User;
using System;
using System.Web.Http;
using System.Web.Mvc;
using Unity.WebApi;

namespace PPF.API
{
    /// <summary>
    /// 
    /// </summary>
    public static class UnityConfig
    { /// <summary>
      /// public single readonly single instance unity container
      /// </summary>
        public static IUnityContainer Container { get; private set; }

        /// <summary>
        /// Registry entry of dependencies
        /// </summary>
        public static void RegisterComponents()
        {
            Container = new UnityContainer();

            // REGISTER ALL YOUR COMPONENTS WITH THE CONTAINER HERE. **** IT IS NOT NECESSARY TO REGISTER YOUR CONTROLLERS

            Container.RegisterType<Func<Type, IServiceBase>>(new InjectionFactory(c => new Func<Type, IServiceBase>(type => (IServiceBase)c.Resolve(type))))
            .RegisterType<Func<string, IServiceBase>>(new InjectionFactory(c => new Func<string, IServiceBase>(name => c.Resolve<IServiceBase>(name))))
            .RegisterType<Func<Type, IModuleBase>>(new InjectionFactory(c => new Func<Type, IModuleBase>(type => (IModuleBase)c.Resolve(type))))
            .RegisterType<IGate, Gate>()
            .RegisterType<IUserModule, UserModule>()
            .RegisterType<IUserService, UserService>()
            .RegisterType<IUserManagerService, UserManagerService>();


            DependencyResolver.SetResolver(new Unity.Mvc5.UnityDependencyResolver(Container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(Container);

        }
    }
}