using EcommerceWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    // Danh sách sản phẩm cho User
    public IActionResult Index()
    {
        var products = _context.Products.Include(p => p.Category).ToList();
        return View(products);
    }

    // Chi tiết sản phẩm cho User
    public IActionResult Details(int id)
    {
        var product = _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.Id == id);

        if (product == null) return NotFound();

        return View(product);
    }
}
