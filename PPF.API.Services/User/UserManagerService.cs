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
        private const string DefaultIdentityProviderClaimValue = "PPF identity";
        public UserManagerService(IUserService userService)
        {
            _userService = userService;

            RoleClaimType = ClaimsIdentity.DefaultRoleClaimType;
            UserIdClaimType = ClaimTypes.NameIdentifier;
            UserNameClaimType = ClaimsIdentity.DefaultNameClaimType;
            SecurityStampClaimType = "AspNet.Identity.SecurityStamp";
            IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
        }


        public string IdentityProviderClaimType { get; private set; }

        public string RoleClaimType { get; private set; }

        public string UserNameClaimType { get; private set; }

        public string UserIdClaimType { get; private set; }

        public string SecurityStampClaimType { get; private set; }

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

        public async Task<Op<Member>> FindAsync(ExternalUserLoginInfo userloginInfo)
        {

            Op<Member> user = await _userService.FindUserExternalLoginInfoAsync(userloginInfo);
            return user;
        }

        public async Task<Op<Member>> FindByNameAsync(string audienceId)
        {
            Op<Member> user = await _userService.FindUserByNameAsync(audienceId);
            if (user.Data == null)
                return new Op<Member>("Invalid User Name", 400, null);
            return new Op<Member>(user.Data);
        }

        public async Task<Op<ClaimsIdentity>> GenerateUserIdentityAsync(Member user, string authenticationType)
        {
            ClaimsIdentity identity = new ClaimsIdentity(authenticationType, UserIdClaimType, RoleClaimType);
            identity.AddClaim(new Claim(UserIdClaimType, user.Id.ToString(), ClaimValueTypes.String));
            identity.AddClaim(new Claim(UserNameClaimType, user.UserName, ClaimValueTypes.String));
            identity.AddClaim(new Claim(IdentityProviderClaimType, DefaultIdentityProviderClaimValue, ClaimValueTypes.String));

            // Adding User Claims
            var userClaims = await _userService.FindUserClaimsAsync(user, authenticationType);
            identity.AddClaims(userClaims.Data);

            // If your DB design still have UserRole Table you can use this part. Fundamentally with new .net Role are converted into Claim
            var userRoles = await _userService.FindUserRolesAsync(user);
            identity.AddClaims(userRoles.Data.Select(r => new Claim(RoleClaimType, r.Name)));

            var userSecurityStamp = await _userService.GetSecurityStampAsync(user);
            identity.AddClaim(new Claim(SecurityStampClaimType, userSecurityStamp.Data));

            return new Op<ClaimsIdentity>(identity);
        }

        public async Task<Op<Member>> CreateAsync(Member user, string password)
        {
            await _userService.UpdateSecurityStampInternalAsync(user);

            var valOp = UserValidator.Validate(user.UserName, password);
            if (!valOp.Succeeded)
                return new Op<Member>(valOp.Meta.Message, valOp.Meta.Code, null);

            var oldResult = await _userService.FindUserByNameAsync(user.UserName);

            if (oldResult.Data != null)
                return new Op<Member>("User name already taken", 400, null);

            var result = await _userService.CreateAsync(user);

            return result;
        }

        public async Task<Op<Member>> CreateExternalLoginAsync(ExternalLogin externalUser, Member user)
        {
            await _userService.UpdateSecurityStampInternalAsync(user);

            var oldResult = await _userService.FindUserByNameAsync(user.UserName);

            if (oldResult.Data != null)
                return new Op<Member>("User name already taken", 400, null);

            Op<Member> result = await _userService.CreateExternalLoginAsync(externalUser, user);

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
