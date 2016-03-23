using PPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPF.API.Repositories
{
    public interface IUserRepository
    {
        Op<long> CreateUser(Member user);
        Op<long> CreateExternalUser(ExternalLogin externalUser);

        Op<Member> FindUserByName(string userName);
        Op<Member> FindUserExternalLoginInfoAsync(ExternalUserLoginInfo userloginInfo);
       
    }
}
