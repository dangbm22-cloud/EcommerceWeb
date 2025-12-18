using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
