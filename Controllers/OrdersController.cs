using LaboGrocery.Data;
using LaboGrocery.Models;
using LaboGrocery.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LaboGrocery.Controllers;

public class OrdersController : Controller
{
    private readonly CartService _cart;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public OrdersController(CartService cart, ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _cart = cart;
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Review()
    {
        var items = _cart.GetCart().ToList();
        if (items.Count == 0) return RedirectToAction("Index", "Cart");

        ViewBag.Total = _cart.Total();
        return View(items); // crea la vista si no la tienes
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Place()
    {
        var items = _cart.GetCart().ToList();
        if (items.Count == 0) return RedirectToAction("Index", "Cart");

        var order = new Order
        {
            CreatedAt = DateTime.UtcNow,
            UserId = _userManager.GetUserId(User),
            Total = items.Sum(i => i.LineTotal)
        };
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        foreach (var i in items)
        {
            _db.OrderLines.Add(new OrderLine
            {
                OrderId = order.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            });
        }
        await _db.SaveChangesAsync();

        _cart.Clear();
        return RedirectToAction(nameof(Thanks));
    }

    public IActionResult Thanks() => View();
}
