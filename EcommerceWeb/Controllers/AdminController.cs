using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Controllers
{
    // Chỉ cho phép tài khoản có role Admin truy cập
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Trang tổng quan Admin
        public IActionResult Dashboard()
        {
            return View();
        }


        // Trang thống kê doanh thu
        public IActionResult Revenue()
        {
            return View();
        }

        // Trang quản lý Danh mục Sản phẩm
        public IActionResult Categories()
        {
            return View();
        }
    }
}
