using Microsoft.AspNetCore.Identity;
using Store.Web.App.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Store.Web.App.services
{
    public class RegistrationService
    {
        private readonly UserManager<User> _userManager;
        public RegistrationService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> RegistrationAsync(UserModel userModel)
        {
            var user = new User();
            user.Email = userModel.Email;
            user.FirstName = userModel.FirstName;
            user.LastName = userModel.LastName;
            user.PhoneNumber = userModel.PhoneNumber.ToString();

            var result = await _userManager.CreateAsync(user, userModel.Password);
                                     
            if(result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "user"));
                return true;
            }
            return false;
        }
    }
}
