using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Orders;

public class DetailsModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public DetailsModel(ShopDbContext db) => _db = db;
    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Orders";
        Order = await _db.Order.Include(o => o.Customer).Include(o => o.OrderDetail).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.OrderID == id);
        return Order == null ? NotFound() : Page();
    }
}
