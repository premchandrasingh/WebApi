using PPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PPF.API.Services.User
{
    public class UserManagerService : IUserManagerService
    {
        IUserService _userService;
        public UserManagerService(IUserService userService)
        {
            _userService = userService;
        }

        public IUserService UserService { get { return _userService; } }

     
        public async Task<Op<Member>> FindAsync(string userName, string password)
        {
            Op<Member> user = await _userService.FindUserByNameAsync(userName);
            if (user.Data == null)
                return new Op<Member>("Invalid User Name", 400, null);

            // Hashing and comparision here. Fow now simply doing base 64 encoding
            if (Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(password)) != Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(user.Data.Password)))
            {
                var invalidRes = await _userService.IncrementAccessFailedCountAsync(user.Data);
                return new Op<Member>(string.Format("Invalid password. {0} attempt left", 5 - invalidRes.Data), 400, null);
            }

            return new Op<Member>(user.Data);
        }


        public async Task<Op<ClaimsIdentity>> GenerateUserIdentityAsync(Member data, string authenticationType)
        {
            var claims = await _userService.FindUserClaimsAsync(data, authenticationType);

            var claimList = claims.Data.ToList();
            claimList.Add(new Claim(ClaimTypes.NameIdentifier, data.UserName));

            ClaimsIdentity identity = new ClaimsIdentity(claimList, authenticationType);

            return new Op<ClaimsIdentity>(identity);
        }

        public async Task<Op<Member>> CreateAsync(Member user, string password)
        {
            await _userService.UpdateSecurityStampInternalAsync(user);

           var valOp = UserValidator.Validate(user.UserName, password);
            if (!valOp.Succeeded)
                return new Op<Member>(valOp.Meta.Message, valOp.Meta.Code, null);

            var result = await _userService.CreateAsync(user);

            return result;
        }

    }


    public class UserValidator
    {
        private static Op<bool> ValidateUserName(string userName)
        {
            foreach (var i in passwordInValidPatterns)
            {
                if (Regex.IsMatch(userName, i.Key))
                {
                    return new Op<bool>(i.Value, 400, false);
                }
            }

            return new Op<bool>(true);
        }

        #region Password validation
        private static Dictionary<string, string> passwordValidPatterns = new Dictionary<string, string>() {
            { "^.{4,8}$", "Password should at least 4 chacter and max 8 character"},
            { "[A-Z]", "Password should contain a capital letter"},
            { "\\d", "Password should contain a number"},
            { "[@#$%^&*()_!]", "Password should contain an special character"},
        };

       private static Dictionary<string, string> passwordInValidPatterns = new Dictionary<string, string>() {
            { "[<>/?\\|+=~`;'\"]", "Invalid special character"},
        };

        private static Op<bool> ValidatePassword(string password)
        {
            foreach (var i in passwordValidPatterns)
            {
                if (!Regex.IsMatch(password, i.Key))
                {
                    return new Op<bool>(i.Value, 400, false);
                }
            }

            foreach (var i in passwordInValidPatterns)
            {
                if (Regex.IsMatch(password, i.Key))
                {
                    return new Op<bool>(i.Value, 400, false);
                }
            }
            return new Op<bool>(true);


        }


        #endregion

        public static Op<bool> Validate(string userName, string password)
        {
            var userVal = ValidateUserName(userName);
            if (!userVal.Succeeded)
                return new Op<bool>(userVal.Meta.Message, userVal.Meta.Code, false);

            var passVal = ValidatePassword(password);
            if (!passVal.Succeeded)
                return new Op<bool>(passVal.Meta.Message, passVal.Meta.Code, false);

            return new Op<bool>(true);
        }
    }
}
