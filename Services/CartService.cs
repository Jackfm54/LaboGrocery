using System.Text.Json;
using LaboGrocery.Data;
using LaboGrocery.Models;
using Microsoft.EntityFrameworkCore;

namespace LaboGrocery.Services;

public class CartLineDto
{
    public int ProductId { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public decimal LineTotal => Price * Quantity;
}

public class CartLineDetail
{
    public Product Product { get; set; } = default!;
    public int Qty { get; set; }
}

public class CartSummary
{
    public List<CartLineDetail> Lines { get; set; } = new();
    public decimal Total { get; set; }
}

public class CartService
{
    private const string KEY = "CART_V1";
    private readonly IHttpContextAccessor _http;
    private readonly ApplicationDbContext _db;

    public CartService(IHttpContextAccessor http, ApplicationDbContext db)
    {
        _http = http;
        _db = db;
    }

    // -------- Persistencia en sesión: Diccionario productId -> cantidad --------
    private Dictionary<int, int> GetMap()
    {
        var json = _http.HttpContext!.Session.GetString(KEY);
        return string.IsNullOrEmpty(json)
            ? new Dictionary<int, int>()
            : JsonSerializer.Deserialize<Dictionary<int, int>>(json)!;
    }

    private void SaveMap(Dictionary<int, int> data)
        => _http.HttpContext!.Session.SetString(KEY, JsonSerializer.Serialize(data));

    // -------- API usada por OrdersController: devuelve líneas con Price/Quantity/ProductId --------
    public IEnumerable<CartLineDto> GetCart()
    {
        var map = GetMap();
        if (map.Count == 0) return Enumerable.Empty<CartLineDto>();

        var ids = map.Keys.ToList();

        // Ajusta "ImagePath" si tu entidad Product usa otra propiedad (p.ej. ImageUrl)
        var products = _db.Products
            .Where(p => ids.Contains(p.Id))
            .Select(p => new { p.Id, p.Name, p.Price, p.ImagePath })
            .ToList();

        return products.Select(p => new CartLineDto
        {
            ProductId = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = map[p.Id],
            ImageUrl = string.IsNullOrWhiteSpace(p.ImagePath)
                ? "/images/placeholder.svg"
                : p.ImagePath
        });
    }

    public int Count() => GetMap().Values.Sum();

    public decimal Total() => GetCart().Sum(l => l.LineTotal);

    // -------- Mutadores síncronos --------
    public void Add(int productId, int qty = 1)
    {
        var map = GetMap();
        if (map.ContainsKey(productId)) map[productId] += qty;
        else map[productId] = qty;
        SaveMap(map);
    }

    public void Remove(int productId)
    {
        var map = GetMap();
        if (map.Remove(productId)) SaveMap(map);
    }

    public void Clear() => SaveMap(new Dictionary<int, int>());

    // -------- Compatibilidad async con controladores --------
    public Task AddAsync(int productId, int qty = 1)
    {
        Add(productId, qty);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(int productId)
    {
        Remove(productId);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        Clear();
        return Task.CompletedTask;
    }

    // -------- Resumen con Product completo (sin tuplas) --------
    public async Task<CartSummary> GetLinesAsync()
    {
        var map = GetMap();
        var ids = map.Keys.ToList();

        var products = await _db.Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();

        var summary = new CartSummary();
        foreach (var p in products)
        {
            var qty = map[p.Id];
            summary.Lines.Add(new CartLineDetail { Product = p, Qty = qty });
            summary.Total += p.Price * qty;
        }
        return summary;
    }
}
