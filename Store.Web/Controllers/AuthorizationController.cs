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
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(AuthorizationModel user)
        {
            if(ModelState.IsValid)
            {
                var isSuccess = await _authorizationService.AuthorizationAsync(user);
                if (isSuccess)
                    return Redirect("/Home/Index");
            }
            return View(user);
        }
    }
}
