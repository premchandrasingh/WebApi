using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPF.Models;

namespace PPF.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        DataPersistance _persist;
        public UserRepository()
        {
            _persist = new DataPersistance();
        }

        public Op<long> CreateUser(Member user)
        {
            var mem = _persist.Create<Member>(Table.Member, user);
            return new Op<long>("User successfully created", user.Id);
        }

        public Op<long> CreateExternalUser(ExternalLogin externalUser)
        {
            var exMem = _persist.Create<ExternalLogin>(Table.ExternalLogin, externalUser);
            return new Op<long>("External user successfully created", externalUser.UserId);
        }

     

        public Op<Member> FindUserByName(string userName)
        {
            var members = _persist.Read<Member>(Table.Member);
            var mem = members.Data.Where(m => m.UserName == userName).FirstOrDefault();
            return new Op<Member>(data: mem);
        }

        public Op<Member> FindUserExternalLoginInfoAsync(ExternalUserLoginInfo userloginInfo)
        {
            var exMembers = _persist.Read<ExternalLogin>(Table.ExternalLogin);
            var exMember = exMembers.Data.FirstOrDefault(m => m.Provider == userloginInfo.LoginProvider && m.ProviderKey == userloginInfo.ProviderKey);

            if (exMember == null)
                return new Op<Member>("No External login found", data: null);

            var members = _persist.Read<Member>(Table.Member);
            var member = members.Data.FirstOrDefault(m => m.Id == exMember.UserId);

            if (member == null)
                return new Op<Member>("Something is not good, external login found but user not found", data: null);

            return new Op<Member>("Member", member);
        }
    }
}
