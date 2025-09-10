using LaboGrocery.Data;
using LaboGrocery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaboGrocery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LanguagesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LanguagesController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.Languages.AsNoTracking().ToListAsync());
        public IActionResult Create() => View(new Language());

        [HttpPost]
        public async Task<IActionResult> Create(Language model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Languages.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
