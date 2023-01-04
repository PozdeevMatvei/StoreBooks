using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Web.App
{
    public class UserRole : IdentityRole<Guid>
    {
        public UserRole() : base()
        { }
        public UserRole(string roleName) : base(roleName)
        { }
    }
}
