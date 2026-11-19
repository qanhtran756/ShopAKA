using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;

namespace ShopAKA.Pages;

public class ProductDetailModel : PageModel
{
    private readonly ShopDbContext _db;

    public ProductDetailModel(ShopDbContext db) => _db = db;

    public Product? Product { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Product = await _db.Product.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductID == id && p.StockQuantity > 0);
        return Product == null ? NotFound() : Page();
    }
}
