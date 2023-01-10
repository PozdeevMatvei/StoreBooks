using Microsoft.AspNetCore.Mvc;
using Store.Web.App.services;

namespace Store.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {           
            return View();
        }
    }
}
