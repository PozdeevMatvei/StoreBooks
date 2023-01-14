using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Web.App.services;

namespace Store.Web.Controllers
{
    [Authorize(Policy = "user")]
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        public AccountController(UserService userService)
        {
            _userService = userService;
        }
        
        public async Task<IActionResult> Index()
        {
            var userModel = await _userService.TryGetUserModelAsync();
            return View(userModel.model);
        }
    }
}
