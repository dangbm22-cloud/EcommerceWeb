using EcommerceWeb.Data;
using EcommerceWeb.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Controllers
{
    public class WishlistController : Controller
    {

        private readonly AppDbContext _context;
        public WishlistController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            //Lấy danh sách ID sp từ session
            var wishlist = HttpContext.Session.GetObjectFromJson<List<int>>("Wishlist") ?? new List<int>();

            //Lấy danh sách sp từ CSDL dựa trên danh sách ID
            var products = _context.Products
                .Where(p => wishlist.Contains(p.Id))
                .ToList();
            return View(products);
        }

        //Thêm sp vào wishlist
        [HttpPost]
        public IActionResult Add(int id)
        {
            var wishlist = HttpContext.Session.GetObjectFromJson<List<int>>("Wishlist") ?? new List<int>();
            if (!wishlist.Contains(id))
            {
                wishlist.Add(id);
                HttpContext.Session.SetObjectAsJson("Wishlist", wishlist);
            }
            return RedirectToAction("Index");

        }

        //Xóa sp khỏi wishlist
        [HttpPost]
        public IActionResult Remove(int id)
        {
            var wishlist = HttpContext.Session.GetObjectFromJson<List<int>>("Wishlist") ?? new List<int>();
            if (wishlist.Contains(id))
            {
                wishlist.Remove(id);
                HttpContext.Session.SetObjectAsJson("Wishlist", wishlist);
            }
            return RedirectToAction("Index");
        }
    }
}
