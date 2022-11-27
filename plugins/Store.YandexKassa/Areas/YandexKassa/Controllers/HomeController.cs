using Microsoft.AspNetCore.Mvc;
using Store.YandexKassa.Areas.YandexKassa.Models;

namespace Store.YandexKassa.Areas.YandexKassa.Controllers
{
    [Area("YandexKassa")]
    public class HomeController : Controller
    {
        [Route("{area}")]
        [Route("{area}/{controller}")]
        [Route("{area}/{controller}/{action}")]
        public IActionResult Index(int orderId, string returnUri)
        {
            return View(new ExampleModel(orderId, returnUri));
        }

        [Route("{area}/{controller}/{action}")]
        public IActionResult Callback(int orderId, string returnUri)
        {
            return View(new ExampleModel(orderId, returnUri, PaymentCompletionOptions.orderPaid));
        }
    }
}
