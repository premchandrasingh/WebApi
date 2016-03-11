using PPF.API.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPF.API.Services
{
    public class ModuleBase
    {
        protected Func<Type, IServiceBase> _serviceFactory;
        protected Func<string, IServiceBase> _namedServiceFactory;

        public ModuleBase(Func<Type, IServiceBase> serviceFactory, Func<string, IServiceBase> namedServiceFactory)
        {
            _serviceFactory = serviceFactory;
            _namedServiceFactory = namedServiceFactory;
        }

        protected T GetService<T>() where T : class, IServiceBase
        {
            try
            {
                T obj = _serviceFactory(typeof(T)) as T;
                if (obj == null)
                {
                    throw new NotSupportedException(string.Format("Dependancy could not resolve. Either '{0}' does not inherit from IServiceBase or '{0}' is not registered in Unity config", typeof(T)));
                }
                return obj;
            }
            catch
            {
                throw new NotSupportedException(string.Format("Dependancy could not resolve. Either '{0}' does not inherit from IServiceBase or '{0}' is not registered in Unity config", typeof(T)));
            }
        }

        protected T GetService<T>(string registerName) where T : class, IServiceBase
        {
            try
            {
                T obj = _namedServiceFactory(registerName) as T;
                if (obj == null)
                {
                    throw new NotSupportedException(string.Format("Dependancy could not resolve. Either '{0}' does not inherit from IServiceBase or '{0}' is not registered in Unity config", typeof(T)));
                }
                return obj;
            }
            catch
            {
                throw new NotSupportedException(string.Format("Dependancy could not resolve. Either '{0}' does not inherit from IServiceBase or '{0}' is not registered in Unity config", typeof(T)));
            }
        }
    }
}
