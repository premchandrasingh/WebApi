using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPF.API.Services.User
{
    public interface IUserModule : IModuleBase
    {
        IUserService UserService { get; }

        IUserManagerService UserManagerService { get; }
    }
}
