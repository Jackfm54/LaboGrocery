using LaboGrocery.Data;
using LaboGrocery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaboGrocery.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Products?category=5
        public async Task<IActionResult> Index(int? category)
        {
            ViewBag.Categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewBag.Category = category;

            var q = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (category.HasValue)
                q = q.Where(p => p.CategoryId == category.Value);

            var list = await q.OrderBy(p => p.Name).ToListAsync();
            return View(list);
        }

        // GET: /Products/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }
    }
}
