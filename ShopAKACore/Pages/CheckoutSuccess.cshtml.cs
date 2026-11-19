using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;

namespace ShopAKA.Pages;

public class CheckoutSuccessModel : PageModel
{
    private readonly ShopDbContext _db;
    public CheckoutSuccessModel(ShopDbContext db) => _db = db;
    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int orderId)
    {
        Order = await _db.Order
            .Include(o => o.OrderDetail).ThenInclude(d => d.Product).ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(o => o.OrderID == orderId);
        return Order == null ? NotFound() : Page();
    }
}
