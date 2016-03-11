using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPF.API.Services.User;

namespace PPF.API.Services
{
    public class Gate : IGate
    {
        private Func<Type, IModuleBase> _moduleFactory;
        public Gate(Func<Type, IModuleBase> moduleFactory)
        {
            _moduleFactory = moduleFactory;
        }

        private IUserModule _userModule = null;
        public IUserModule UserModule
        {
            get
            {
                if (_userModule == null)
                    _userModule = GetModule<IUserModule>();
                return _userModule;
            }
        }



        private T GetModule<T>() where T : class, IModuleBase
        {
            try
            {
                T obj = _moduleFactory(typeof(T)) as T;
                if (obj == null)
                {
                    throw new NotSupportedException(string.Format("Dependancy could not resolve. Either '{0}' does not inherit from IModuleBase or '{0}' is not registered in Unity config", typeof(T)));
                }
                return obj;
            }
            catch
            {
                throw new NotSupportedException(string.Format("Dependancy could not resolve. Either '{0}' does not inherit from IModuleBase or '{0}' is not registered in Unity config", typeof(T)));
            }
        }
    }
}
