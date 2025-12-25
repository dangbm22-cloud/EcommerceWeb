using EcommerceWeb.Data;
using EcommerceWeb.Helpers;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        public IActionResult Cart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        public IActionResult Add(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.Product.Id == id);

            if (item != null)
                item.Quantity++;
            else
                cart.Add(new CartItem { Product = product, Quantity = 1 });

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Cart");
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.Product.Id == id);

            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Cart");
        }

        // cập nhật số lượng bằng AJAX
        [HttpPost]
        public IActionResult UpdateQuantityAjax(int id, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.Product.Id == id);

            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            var itemTotal = item?.Product.Price * item?.Quantity ?? 0;
            var cartTotal = cart.Sum(i => i.Product.Price * i.Quantity);

            return Json(new { itemTotal, cartTotal });
        }
    }
}
