using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }
    }
}
