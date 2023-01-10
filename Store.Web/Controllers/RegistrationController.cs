using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Store.Web.App.models;
using Store.Web.App.services;

namespace Store.Web.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly RegistrationService _registrationService;
        public RegistrationController(RegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(RegistrationModel user)
        {
            if(ModelState.IsValid)
            {
                var isSuccess = await _registrationService.RegistrationAsync(user);
                if (isSuccess)
                    return View("SuccessRegistration");
            }
            return View(user);
        }
    }
}
