using LaboGrocery.Data;
using LaboGrocery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaboGrocery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProductsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db; _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.Include(p => p.Category).AsNoTracking().ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.AsNoTracking().ToListAsync();
            return View(new Product { Price = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.Categories.AsNoTracking().ToListAsync();
                return View(model);
            }

            if (image != null && image.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "images", "products");
                Directory.CreateDirectory(folder);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var path = Path.Combine(folder, fileName);
                using var fs = new FileStream(path, FileMode.Create);
                await image.CopyToAsync(fs);
                model.ImagePath = $"/images/products/{fileName}";
            }

            _db.Products.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = await _db.Categories.AsNoTracking().ToListAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product model, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.Categories.AsNoTracking().ToListAsync();
                return View(model);
            }

            var prod = await _db.Products.FindAsync(model.Id);
            if (prod == null) return NotFound();

            prod.Name = model.Name;
            prod.Description = model.Description;
            prod.Price = model.Price;
            prod.CategoryId = model.CategoryId;

            if (image != null && image.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "images", "products");
                Directory.CreateDirectory(folder);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var path = Path.Combine(folder, fileName);
                using var fs = new FileStream(path, FileMode.Create);
                await image.CopyToAsync(fs);
                prod.ImagePath = $"/images/products/{fileName}";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var prod = await _db.Products.FindAsync(id);
            if (prod != null) { _db.Products.Remove(prod); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}
