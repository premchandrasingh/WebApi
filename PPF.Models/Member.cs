using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPF.Models
{
    public class Member
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string SecurityStamp { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsExternal { get; set; }
    }
}
