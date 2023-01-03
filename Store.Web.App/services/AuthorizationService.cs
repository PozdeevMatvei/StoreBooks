using Microsoft.AspNetCore.Identity;
using Store.Web.App.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Web.App.services
{
    public class AuthorizationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthorizationService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> AuthorizationAsync(AuthorizationModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if(user == null)
                return false;

            var result = await _signInManager
                .PasswordSignInAsync(user, loginModel.Password, false, false);

            if(result.Succeeded)
                return true;

            return false;
            // TODO сделать контроллеры и представления по регистрации, авторизации
        }
    }
}
