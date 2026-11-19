using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ShopAKA.Models;

namespace ShopAKA.Services;

public class CartService
{
    private const string CartKey = "ShopCart";
    private readonly IHttpContextAccessor _http;

    public CartService(IHttpContextAccessor http) => _http = http;

    public List<CartItem> GetCart()
    {
        var json = _http.HttpContext?.Session.GetString(CartKey);
        if (string.IsNullOrEmpty(json)) return new List<CartItem>();
        return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    public void SaveCart(List<CartItem> items)
    {
        _http.HttpContext?.Session.SetString(CartKey, JsonSerializer.Serialize(items));
    }

    public int GetTotalQuantity() => GetCart().Sum(i => i.Quantity);

    public void ClearCart() => _http.HttpContext?.Session.Remove(CartKey);
}
