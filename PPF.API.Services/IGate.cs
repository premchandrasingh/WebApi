using PPF.API.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPF.API.Services
{
    /// <summary>
    /// Gate service is the only single access point thats why its name is GATE
    /// </summary>
    public interface IGate : IServiceBase
    {
        IUserModule UserModule { get; }
    }
}
