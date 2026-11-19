using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Orders;

public class IndexModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public IndexModel(ShopDbContext db) => _db = db;
    public IList<Order> Orders { get; set; } = new List<Order>();

    public async Task<IActionResult> OnGetAsync()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Orders";
        Orders = await _db.Order.Include(o => o.Customer).OrderByDescending(o => o.OrderDate).ToListAsync();
        return Page();
    }
}
