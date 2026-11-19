using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;

namespace ShopAKA.Pages;

public class CategoryProductsModel : PageModel
{
    private readonly ShopDbContext _db;
    public CategoryProductsModel(ShopDbContext db) => _db = db;

    public string CategoryName { get; set; } = "";
    public List<Product> Products { get; set; } = new();
    public int CategoryId { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;

    public async Task<IActionResult> OnGetAsync(int id, int page = 1)
    {
        var category = await _db.Category.FindAsync(id);
        if (category == null) return NotFound();
        CategoryId = id;
        CategoryName = category.CategoryName;
        CurrentPage = page;
        const int pageSize = 12;
        var query = _db.Product.Where(p => p.CategoryID == id && p.StockQuantity > 0);
        var total = await query.CountAsync();
        TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        Products = await query.OrderBy(p => p.ProductID).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Page();
    }
}
