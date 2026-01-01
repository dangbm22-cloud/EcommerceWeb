using EcommerceWeb.Data;
using EcommerceWeb.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;

public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    // Trang Shop: danh sách sản phẩm với tìm kiếm, lọc theo danh mục và giá
    public IActionResult Index(string search, string category, decimal? minPrice, decimal? maxPrice)
    {
        var products = _context.Products.Include(p => p.Category).AsQueryable();

        // Tìm theo tên
        if (!string.IsNullOrEmpty(search))
        {
            products = products.Where(p => p.Name.Contains(search));
        }

        // Lọc theo danh mục
        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category.Name == category);
        }

        // Lọc theo giá
        if (minPrice.HasValue)
        {
            products = products.Where(p => p.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= maxPrice.Value);
        }

        return View(products.ToList());
    }

    // Trang chi tiết sản phẩm
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


}
