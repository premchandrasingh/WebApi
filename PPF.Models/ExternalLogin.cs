using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPF.Models
{
    public class ExternalLogin : IModel
    {
        public string Provider { get; set; }

        public string ProviderKey { get; set; }

        public long UserId { get; set; }
    }
}
