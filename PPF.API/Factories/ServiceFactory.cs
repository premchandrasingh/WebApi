using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using PPF.API.Services;
using PPF.API.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;

namespace PPF.API
{
    /// <summary>
    /// Get api service on demand
    /// </summary>
    public class ServiceFactory : IDisposable
    {
        private Func<Type, IServiceBase> _serviceFactory;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceFactory"></param>
        public ServiceFactory(Func<Type, IServiceBase> serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() where T : IServiceBase
        {
            return (T)_serviceFactory(typeof(T));
        }

        /// <summary>
        /// Create log service context
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ServiceFactory Create(IdentityFactoryOptions<ServiceFactory> options, IOwinContext context)
        {
            var obj = new ServiceFactory(type => (IServiceBase)UnityConfig.Container.Resolve(type));
            context.Set(obj);
            return obj;
        }

        /// <summary>
        /// Dispose the context
        /// </summary>
        public void Dispose()
        {
            _serviceFactory = null;
        }
    }
}