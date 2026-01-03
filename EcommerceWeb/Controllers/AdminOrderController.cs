using EcommerceWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWeb.Controllers
{
    [Authorize(Roles = "Admin")] // chỉ Admin mới vào được
    public class AdminOrderController : Controller
    {
        private readonly AppDbContext _context;

        public AdminOrderController(AppDbContext context)
        {
            _context = context;
        }

        // Danh sách đơn hàng
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.Details)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders);
        }

        // Chi tiết đơn hàng
        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Product) // thông tin từ Product
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            // Nếu đơn đã kết thúc thì không cho cập nhật
            if (order.Status == "Completed"
                || order.Status == "CancelledByAdmin"
                || order.Status == "CancelledByCustomer")
            {
                TempData["Error"] = "Đơn hàng đã kết thúc, không thể cập nhật trạng thái.";
                return RedirectToAction("Details", new { id });
            }

            // Nếu đơn đã Shipped thì không cho hủy nữa
            if (order.Status == "Shipped" && status == "CancelledByAdmin")
            {
                TempData["Error"] = "Đơn hàng đã giao cho đơn vị vận chuyển, không thể hủy.";
                return RedirectToAction("Details", new { id });
            }

            // Cập nhật trạng thái hợp lệ
            order.Status = status;
            _context.SaveChanges();

            return RedirectToAction("Details", new { id });
        }


    }
}
