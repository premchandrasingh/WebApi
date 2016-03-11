using System;

namespace PPF.API.Services.User
{
    public class UserModule : ModuleBase, IUserModule
    {
        
        public UserModule(Func<Type, IServiceBase> serviceFactory, Func<string, IServiceBase> namedServiceFactory)
            :base(serviceFactory, namedServiceFactory)
        { }

        private IUserService _userService = null;
        public IUserService UserService
        {
            get
            {
                if (_userService == null)
                {
                    // Open this to route logging into inbuilt service
                    _userService = GetService<IUserService>() as IUserService;
                }
                return _userService;
            }
        }


        private IUserManagerService _userMngService = null;
        public IUserManagerService UserManagerService
        {
            get
            {
                if (_userMngService == null)
                {
                    // Open this to route logging into inbuilt service
                    _userMngService = GetService<IUserManagerService>() as IUserManagerService;
                }
                return _userMngService;
            }
        }

    }


}