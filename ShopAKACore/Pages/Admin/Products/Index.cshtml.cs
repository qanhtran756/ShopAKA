using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Products;

public class IndexModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public IndexModel(ShopDbContext db) => _db = db;
    public IList<Product> Products { get; set; } = new List<Product>();

    public async Task<IActionResult> OnGetAsync()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Products";
        Products = await _db.Product.Include(p => p.Category).OrderBy(p => p.ProductID).ToListAsync();
        return Page();
    }
}
