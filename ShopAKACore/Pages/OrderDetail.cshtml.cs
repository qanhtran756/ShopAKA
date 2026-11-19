using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Services;

namespace ShopAKA.Pages;

public class OrderDetailModel : PageModel
{
    private readonly ShopDbContext _db;
    private readonly AuthService _auth;

    public OrderDetailModel(ShopDbContext db, AuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!_auth.IsCustomerLoggedIn) return RedirectToPage("/Login");

        Order = await _db.Order
            .Include(o => o.OrderDetail).ThenInclude(d => d.Product).ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(o => o.OrderID == id && o.CustomerID == _auth.CustomerId);

        return Order == null ? NotFound() : Page();
    }
}
