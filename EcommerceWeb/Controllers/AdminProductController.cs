using EcommerceWeb.Data;
using EcommerceWeb.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace EcommerceWeb.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ cho phép tài khoản role Admin truy cập 
    public class AdminProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Danh sách sản phẩm
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        // Chi tiết sản phẩm
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // Tạo sản phẩm mới (GET)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();

            var vm = new ProductViewModel
            {
                Specifications = new List<SpecificationItem> { new() }
            };

            return View(vm);
        }

        // Tạo sản phẩm mới (POST + upload ảnh)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel vm, IFormFile? ImageFile)
        {
            ViewBag.Categories = _context.Categories.ToList();

            if (!ModelState.IsValid) return View(vm);

            var product = new Product
            {
                Name = vm.Name,
                Price = vm.Price,
                Description = vm.Description,
                Stock = vm.Stock,
                CategoryId = vm.CategoryId,
                ImageUrl = vm.ImageUrl,
                Specifications = JsonConvert.SerializeObject(vm.Specifications)
            };

            // Xử lý ảnh
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Phần sửa sản phẩm (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImageFile)
        {
            if (id != product.Id) return NotFound();

            var category = await _context.Categories.FindAsync(product.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError(nameof(product.CategoryId), "Danh mục không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View(product);
            }

            try
            {
                // Nếu có upload ảnh mới
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    const long maxBytes = 5 * 1024 * 1024;
                    if (ImageFile.Length > maxBytes)
                    {
                        ModelState.AddModelError("ImageFile", "Ảnh quá lớn, tối đa 5MB.");
                        ViewBag.Categories = _context.Categories.ToList();
                        return View(product);
                    }

                    var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
                    if (!allowed.Contains(ImageFile.ContentType))
                    {
                        ModelState.AddModelError("ImageFile", "Định dạng ảnh không hỗ trợ (chỉ JPG, PNG, WEBP).");
                        ViewBag.Categories = _context.Categories.ToList();
                        return View(product);
                    }

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
                        await ImageFile.CopyToAsync(stream);
                    }

                    product.ImageUrl = $"/img/product/{categoryFolderName}/{safeFileName}";
                }
                else // Nếu không upload ảnh mới, giữ nguyên ảnh cũ
                {
                    var oldProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    product.ImageUrl = oldProduct?.ImageUrl;
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == product.Id))
                    return NotFound();
                else
                    throw;
            }
        }


        // Xóa sản phẩm
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

        // Hàm tạo tên hợp lệ cho file ảnh để đưa luôn ảnh vào bài
        private static string Slugify(string input)
        {
            input = input.Trim().ToLowerInvariant();
            input = Regex.Replace(input, @"[^a-z0-9\-]+", "-");
            input = Regex.Replace(input, @"-+", "-");
            return input.Trim('-');
        }
    }
}
