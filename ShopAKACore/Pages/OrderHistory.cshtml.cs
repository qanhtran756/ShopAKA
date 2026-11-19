using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Services;

namespace ShopAKA.Pages;

public class OrderHistoryModel : PageModel
{
    private readonly ShopDbContext _db;
    private readonly AuthService _auth;

    public OrderHistoryModel(ShopDbContext db, AuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    public List<Order> Orders { get; set; } = new();

    public IActionResult OnGet()
    {
        if (!_auth.IsCustomerLoggedIn) return RedirectToPage("/Login");
        Orders = _db.Order.Where(o => o.CustomerID == _auth.CustomerId)
            .OrderByDescending(o => o.OrderDate).ToList();
        return Page();
    }
}
