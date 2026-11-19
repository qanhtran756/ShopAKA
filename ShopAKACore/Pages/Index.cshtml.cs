using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;

namespace ShopAKA.Pages;

public class IndexModel : PageModel
{
    private readonly ShopDbContext _db;

    public IndexModel(ShopDbContext db) => _db = db;

    public List<CategoryWithProductsViewModel> CategoriesWithProducts { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;

    public async Task OnGetAsync(int page = 1)
    {
        const int categoriesPerPage = 2;
        CurrentPage = page;
        var totalCategories = await _db.Category.CountAsync();
        TotalPages = Math.Max(1, (int)Math.Ceiling(totalCategories / (double)categoriesPerPage));

        var categories = await _db.Category.OrderBy(c => c.CategoryID)
            .Skip((page - 1) * categoriesPerPage).Take(categoriesPerPage).ToListAsync();

        CategoriesWithProducts = categories.Select(c => new CategoryWithProductsViewModel
        {
            Category = c,
            Products = _db.Product.Where(p => p.CategoryID == c.CategoryID && p.StockQuantity > 0)
                .OrderBy(p => p.ProductID).ToList()
        }).ToList();
    }
}
