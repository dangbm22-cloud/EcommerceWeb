using EcommerceWeb.Data;
using EcommerceWeb.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        // Chi tiết sản phẩm (hiển thị Specifications)
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            var specs = string.IsNullOrEmpty(product.Specifications)
                ? new List<SpecificationItem>()
                : JsonConvert.DeserializeObject<List<SpecificationItem>>(product.Specifications);

            ViewBag.Specifications = specs;
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

        // Tạo sản phẩm mới (POST + upload ảnh + Specifications)
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

            // Xử lý upload ảnh
            if (ImageFile != null && ImageFile.Length > 0)
            {
                const long maxBytes = 5 * 1024 * 1024;
                if (ImageFile.Length > maxBytes)
                {
                    ModelState.AddModelError("ImageFile", "Ảnh quá lớn, tối đa 5MB.");
                    return View(vm);
                }

                var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
                if (!allowed.Contains(ImageFile.ContentType))
                {
                    ModelState.AddModelError("ImageFile", "Định dạng ảnh không hỗ trợ (chỉ JPG, PNG, WEBP).");
                    return View(vm);
                }

                var webRoot = _env.WebRootPath;
                var categoryFolderName = Slugify(_context.Categories.Find(vm.CategoryId)!.Name);
                var folderPath = Path.Combine(webRoot, "img", "product", categoryFolderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var extension = Path.GetExtension(ImageFile.FileName);
                var baseName = Slugify(vm.Name);
                var safeFileName = $"{baseName}-{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folderPath, safeFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                product.ImageUrl = $"/img/product/{categoryFolderName}/{safeFileName}";
            }

            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Sửa sản phẩm (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var vm = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                Specifications = string.IsNullOrEmpty(product.Specifications)
                    ? new List<SpecificationItem>()
                    : JsonConvert.DeserializeObject<List<SpecificationItem>>(product.Specifications)
            };

            ViewBag.Categories = _context.Categories.ToList();
            return View(vm);
        }

        // Sửa sản phẩm (POST + Specifications + upload ảnh)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel vm, IFormFile? ImageFile)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View(vm);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = vm.Name;
            product.Price = vm.Price;
            product.Description = vm.Description;
            product.Stock = vm.Stock;
            product.CategoryId = vm.CategoryId;
            product.Specifications = JsonConvert.SerializeObject(vm.Specifications);

            // Nếu có upload ảnh mới
            if (ImageFile != null && ImageFile.Length > 0)
            {
                const long maxBytes = 5 * 1024 * 1024;
                if (ImageFile.Length > maxBytes)
                {
                    ModelState.AddModelError("ImageFile", "Ảnh quá lớn, tối đa 5MB.");
                    return View(vm);
                }

                var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
                if (!allowed.Contains(ImageFile.ContentType))
                {
                    ModelState.AddModelError("ImageFile", "Định dạng ảnh không hỗ trợ (chỉ JPG, PNG, WEBP).");
                    return View(vm);
                }

                var webRoot = _env.WebRootPath;
                var categoryFolderName = Slugify(_context.Categories.Find(vm.CategoryId)!.Name);
                var folderPath = Path.Combine(webRoot, "img", "product", categoryFolderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var extension = Path.GetExtension(ImageFile.FileName);
                var baseName = Slugify(vm.Name);
                var safeFileName = $"{baseName}-{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folderPath, safeFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                product.ImageUrl = $"/img/product/{categoryFolderName}/{safeFileName}";
            }
            else
            {
                // Nếu không upload ảnh mới
                if (!string.IsNullOrEmpty(vm.ImageUrl))
                {
                    product.ImageUrl = vm.ImageUrl; // nếu Admin nhập URL mới
                }
                // nếu vm.ImageUrl rỗng thì giữ nguyên ảnh cũ
            }

            _context.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

        // Hàm tạo tên hợp lệ cho file ảnh
        private static string Slugify(string input)
        {
            input = input.Trim().ToLowerInvariant();
            input = Regex.Replace(input, @"[^a-z0-9\-]+", "-");
            input = Regex.Replace(input, @"-+", "-");
            return input.Trim('-');
        }
    }
}