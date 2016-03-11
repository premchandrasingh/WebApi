using PPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PPF.API.Services.User
{
    public class UserService : IUserService
    {

        private List<Member> _members = new List<Member>() {
            new Member() { Id = 1, UserName = "Prem1", Password ="1234", FirstName ="Prem1", LastName ="Singh", Email ="prem@email.com" },
            new Member() { Id = 2, UserName = "Prem2", Password ="1234", FirstName ="Prem2", LastName ="Singh", Email ="prem@email.com" },
            new Member() { Id = 3, UserName = "Prem3", Password ="1234", FirstName ="Prem3", LastName ="Singh", Email ="prem@email.com" },
            new Member() { Id = 4, UserName = "Prem4", Password ="1234", FirstName ="Prem4", LastName ="Singh", Email ="prem@email.com" },
            new Member() { Id = 5, UserName = "Prem5", Password ="1234", FirstName ="Prem5", LastName ="Singh", Email ="prem@email.com" }
        };

        public async Task<Op<int>> IncrementAccessFailedCountAsync(Member user)
        {
            return new Op<int>(2);
        }

        public Task<Op<Member>> FindUserByNameAsync(string userName)
        {
            var user = _members.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            return Task.FromResult(new Op<Member>(user));
        }


        public Task<Op<IEnumerable<Claim>>> FindUserClaimsAsync(Member data, string authenticationType)
        {

            IEnumerable<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Role, "User")
            };
            return Task.FromResult(new Op<IEnumerable<Claim>>(claims));
        }

        public async Task<Op<bool>> UpdateSecurityStampInternalAsync(Member user)
        {
            // Update Security time stamp like Guid.NewGuid()
            return new Op<bool>(true);
        }

        public async Task<Op<Member>> CreateAsync(Member user)
        {
            user.Id = (new Random()).Next();
            return new Op<Member>(user);
        }
    }

}
