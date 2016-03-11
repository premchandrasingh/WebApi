using System.Security.Claims;
using System.Threading.Tasks;
using PPF.Models;

namespace PPF.API.Services.User
{
    public interface IUserManagerService : IServiceBase
    {
        IUserService UserService { get; }

        Task<Op<Member>> CreateAsync(Member user, string password);
        Task<Op<Member>> FindAsync(string userName, string password);
        Task<Op<ClaimsIdentity>> GenerateUserIdentityAsync(Member data, string authenticationType);
    }
}