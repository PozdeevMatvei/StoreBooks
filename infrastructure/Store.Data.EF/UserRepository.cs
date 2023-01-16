using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Store.Web.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DTO.EF
{
    public class UserRepository : IUserRepository
    {
        private readonly StoreDbContextFactory _dbContextFactory;
        public UserRepository(StoreDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<User> GetUserAsync(string userName)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(UserRepository));          
            var user = await dbContext.Users
                                      .Include(user => user.Orders)
                                      .ThenInclude(order => order.Items)
                                      .SingleAsync(user => user.UserName == userName);
            return user;
        }
        public async Task UpdateAsync()
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(UserRepository));
            await dbContext.SaveChangesAsync();
        }
    }
}
