using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPF.Models
{
    public class ExternalUserLoginInfo
    {
        public ExternalUserLoginInfo(string loginProvider, string providerKey)
        {
            this.LoginProvider = loginProvider;
            this.ProviderKey = providerKey;
        }

        //
        // Summary:
        //     Provider for the linked login, i.e. Facebook, Google, etc.
        public string LoginProvider { get; set; }

        //
        // Summary:
        //     User specific key for the login provider
        public string ProviderKey { get; set; }
    }
}
