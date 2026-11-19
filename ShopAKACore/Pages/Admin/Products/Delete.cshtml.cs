using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Products;

public class DeleteModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public DeleteModel(ShopDbContext db) => _db = db;
    public Product? Product { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Products";
        Product = await _db.Product.Include(p => p.OrderDetail).FirstOrDefaultAsync(p => p.ProductID == id);
        return Product == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        var product = await _db.Product.Include(p => p.OrderDetail).FirstOrDefaultAsync(p => p.ProductID == id);
        if (product == null) return NotFound();
        if (product.OrderDetail.Any()) { TempData["Error"] = "Có đơn hàng liên quan"; return RedirectToPage("Index"); }
        _db.Product.Remove(product);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
