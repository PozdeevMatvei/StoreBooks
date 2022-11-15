using Microsoft.AspNetCore.Mvc;

namespace Store.YandexKassa.Areas.YandexKassa.Controllers
{
    [Area("YandexKassa")]
    public class HomeController : Controller
    {
        [Route("{area}")]
        [Route("{area}/{controller}")]
        [Route("{area}/{controller}/{action}")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("{area}/{controller}/{action}")]
        public IActionResult Callback()
        {
            return View();
        }
    }
}
