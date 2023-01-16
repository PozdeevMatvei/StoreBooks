using Store.Web.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(string userName);
        Task UpdateAsync();
    }
}
