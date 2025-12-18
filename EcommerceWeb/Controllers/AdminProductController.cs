using System.IO;
using System.Text.RegularExpressions;
using EcommerceWeb.Data;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWeb.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
    public class AdminProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 📌 Danh sách sản phẩm
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        // 📌 Chi tiết sản phẩm
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // 📌 Tạo sản phẩm mới (GET)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // 📌 Tạo sản phẩm mới (POST + upload ảnh)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product, IFormFile? ImageFile)
        {
            ViewBag.Categories = _context.Categories.ToList();

            var category = _context.Categories.Find(product.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError(nameof(product.CategoryId), "Danh mục không hợp lệ.");
            }

            // Nếu cả hai đều trống thì báo lỗi
            if ((ImageFile == null || ImageFile.Length == 0) && string.IsNullOrEmpty(product.ImageUrl))
            {
                ModelState.AddModelError("ImageUrl", "Bạn phải upload ảnh hoặc nhập URL.");
            }

            // Nếu có upload ảnh thì kiểm tra dung lượng và định dạng
            if (ImageFile != null && ImageFile.Length > 0)
            {
                const long maxBytes = 5 * 1024 * 1024;
                if (ImageFile.Length > maxBytes)
                {
                    ModelState.AddModelError("ImageFile", "Ảnh quá lớn, tối đa 5MB.");
                }

                var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
                if (!allowed.Contains(ImageFile.ContentType))
                {
                    ModelState.AddModelError("ImageFile", "Định dạng ảnh không hỗ trợ (chỉ JPG, PNG, WEBP).");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList(); // Bắt buộc phải gán lại
                return View(product);
            }

            // Nếu có upload ảnh thì lưu file và gán ImageUrl
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var webRoot = _env.WebRootPath;
                var categoryFolderName = Slugify(category!.Name);
                var folderPath = Path.Combine(webRoot, "img", "product", categoryFolderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var extension = Path.GetExtension(ImageFile.FileName);
                var baseName = Slugify(product.Name);
                var safeFileName = $"{baseName}-{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folderPath, safeFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                product.ImageUrl = $"/img/product/{categoryFolderName}/{safeFileName}";
            }
            // Nếu không upload ảnh nhưng có nhập URL thì xài ImageUrl
            // (không cần xử lý thêm)

            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // 📌 Sửa sản phẩm (GET)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // 📌 Sửa sản phẩm (POST + upload ảnh mới)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product, IFormFile? ImageFile)
        {
            ViewBag.Categories = _context.Categories.ToList();

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var category = _context.Categories.Find(product.CategoryId);

            if (ImageFile != null && ImageFile.Length > 0 && category != null)
            {
                var webRoot = _env.WebRootPath;
                var categoryFolderName = Slugify(category.Name);
                var folderPath = Path.Combine(webRoot, "img", "product", categoryFolderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var extension = Path.GetExtension(ImageFile.FileName);
                var baseName = Slugify(product.Name);
                var safeFileName = $"{baseName}-{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folderPath, safeFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                product.ImageUrl = $"/img/product/{categoryFolderName}/{safeFileName}";
            }

            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // 📌 Xóa sản phẩm
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            // Xóa cả file ảnh trong wwwroot
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var filePath = Path.Combine(_env.WebRootPath, product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // 📌 Hàm tạo slug an toàn cho tên file/thư mục
        private static string Slugify(string input)
        {
            input = input.Trim().ToLowerInvariant();
            input = Regex.Replace(input, @"[^a-z0-9\-]+", "-");
            input = Regex.Replace(input, @"-+", "-");
            return input.Trim('-');
        }
    }
}
