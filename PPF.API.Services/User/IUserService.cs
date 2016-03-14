using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using PPF.Models;

namespace PPF.API.Services.User
{
    public interface IUserService : IServiceBase
    {
        Task<Op<Member>> CreateAsync(Member user);
        Task<Op<Member>> FindUserByNameAsync(string userName);
        Task<Op<IEnumerable<Claim>>> FindUserClaimsAsync(Member data, string authenticationType);
        Task<Op<int>> IncrementAccessFailedCountAsync(Member user);
        Task<Op<bool>> UpdateSecurityStampInternalAsync(Member user);
        Task<Op<IEnumerable<Role>>> FindUserRolesAsync(Member user);
        Task<Op<string>> GetSecurityStampAsync(Member user);
    }
}