using EcommerceWeb.Data;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcommerceWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var featuredProducts = _context.Products
                .Include(p => p.Category)
                .Take(8)
                .ToList();

            var latestProducts = _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToList();

            ViewBag.Featured = featuredProducts;
            ViewBag.Latest = latestProducts;

            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
