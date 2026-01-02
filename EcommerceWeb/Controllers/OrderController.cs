using EcommerceWeb.Data;
using EcommerceWeb.Helpers;
using EcommerceWeb.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWeb.Controllers
{
    [Authorize] // bắt buộc đăng nhập
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // --- CART ---
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

        // --- CHECKOUT ---
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (!cart.Any()) return RedirectToAction("Cart");

            var vm = new CheckoutViewModel { CartItems = cart };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutViewModel vm)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (!cart.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng trống.");
                vm.CartItems = cart;
                return View(vm);
            }

            if (!ModelState.IsValid)
            {
                vm.CartItems = cart;
                return View(vm);
            }

            //Tính lại subtotal và total từ giỏ hàng
            var subtotal = cart.Sum(c => c.Product.Price * c.Quantity);
            var total = subtotal; // cái này note lại để sau này có thêm phí ship, giảm giá

            var order = new Order
            {
                UserId = User.Identity?.Name,
                FullName = $"{vm.FirstName} {vm.LastName}".Trim(),
                Phone = vm.Phone,
                Email = vm.Email,
                Address = vm.Address,
                City = vm.City,
                Notes = vm.Notes,
                PaymentMethod = vm.PaymentMethod,
                Status = "Pending",
                Subtotal = subtotal,
                Total = total,
                CreatedAt = DateTime.UtcNow // đảm bảo ngày tạo đơn
            };

            order.Details = cart.Select(c => new OrderDetail
            {
                ProductId = c.Product.Id,
                ProductName = c.Product.Name,
                UnitPrice = c.Product.Price,
                Quantity = c.Quantity
            }).ToList();

            _context.Orders.Add(order);
            _context.SaveChanges();

            HttpContext.Session.SetObjectAsJson("Cart", new List<CartItem>()); // làm trống giỏ hàng cart khi đã xác nhận đặt hàng

            return RedirectToAction("Success", new { id = order.Id });
        }

        // Thông báo cho người ta biết đã đặt thành công
        [HttpGet]
        public IActionResult Success(int id)
        {
            var order = _context.Orders
                .Include(o => o.Details)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();

            // bảo mật: chỉ xem được đơn hàng của bản thân
            if (order.UserId != User.Identity?.Name)
                return Forbid();
            return View(order); // Success.cshtml
        }

        // Đơn hàng
        [HttpGet]
        public IActionResult Status(int id)
        {
            var order = _context.Orders
                .Include(o => o.Details)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();

            // chỉ xem được đơn hàng của bản thân
            if (order.UserId != User.Identity?.Name)
                return Forbid();

            return View(order); // Status.cshtml
        }


        //Xem danh sách đơn hàng của user
        [HttpGet]
        public IActionResult MyOrders()
        {
            var userId = User.Identity?.Name;
            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders); // MyOrders.cshtml
        }
    }
}
