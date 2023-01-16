using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Web.App.models;
using Store.Web.App.services;

namespace Store.Web.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly AuthorizationService _authorizationService;
        public AuthorizationController(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(AuthorizationModel user)
        {
            if(ModelState.IsValid)
            {
                var isSuccess = await _authorizationService.LogInAsync(user);
                if (isSuccess)
                    return Redirect("/Home/Index");
            }
            return View(user);
        }
        [Authorize(Policy = "user")]
        public async Task<IActionResult> LogOut()
        {
            await _authorizationService.LogOutAsync();
            return Redirect("/Home/Index");
        }
    }
}
