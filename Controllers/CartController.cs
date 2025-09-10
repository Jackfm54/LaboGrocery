using Microsoft.AspNetCore.Mvc;
using LaboGrocery.Services;

namespace LaboGrocery.Controllers;

public class CartController : Controller
{
    private readonly CartService _cart;

    public CartController(CartService cart)
    {
        _cart = cart;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var summary = await _cart.GetLinesAsync(); // CartSummary
        return View(summary); // La vista puede tiparse a CartSummary
    }

    [HttpPost]
    public async Task<IActionResult> Add(int id, int qty = 1)
    {
        await _cart.AddAsync(id, qty);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        await _cart.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        await _cart.ClearAsync();
        return RedirectToAction(nameof(Index));
    }
}
