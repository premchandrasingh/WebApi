using System.Security.Claims;
using System.Threading.Tasks;
using PPF.Models;

namespace PPF.API.Services.User
{
    public interface IUserManagerService : IServiceBase
    {
        IUserService UserService { get; }

        string RoleClaimType { get; }
        string UserNameClaimType { get; }
        string UserIdClaimType { get; }
        string SecurityStampClaimType { get; }

        Task<Op<Member>> CreateAsync(Member user, string password);
        Task<Op<Member>> FindAsync(string userName, string password);
        Task<Op<Member>> FindAsync(ExternalUserLoginInfo userloginInfo);



        Task<Op<ClaimsIdentity>> GenerateUserIdentityAsync(Member data, string authenticationType);
        Task<Op<Member>> FindByNameAsync(string audienceId);
        Task<Op<Member>> CreateExternalLoginAsync(ExternalLogin externalLogin, Member member);
    }
}