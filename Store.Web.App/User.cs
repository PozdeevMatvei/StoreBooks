using Microsoft.AspNetCore.Identity;
using Store.Web.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public IEnumerable<Order>? Orders { get; set; }
    }
}
