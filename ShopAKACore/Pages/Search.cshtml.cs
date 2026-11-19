using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;

namespace ShopAKA.Pages;

public class SearchModel : PageModel
{
    private readonly ShopDbContext _db;
    public SearchModel(ShopDbContext db) => _db = db;

    public string Query { get; set; } = "";
    public List<Product> Products { get; set; } = new();
    public int TotalResults { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }

    public async Task OnGetAsync(string? query, int page = 1)
    {
        Query = query ?? "";
        CurrentPage = page;
        if (string.IsNullOrWhiteSpace(Query)) return;

        const int pageSize = 12;
        var q = _db.Product.Include(p => p.Category)
            .Where(p => p.StockQuantity > 0 &&
                (p.ProductName.Contains(Query) || (p.ProductDecription != null && p.ProductDecription.Contains(Query))));

        TotalResults = await q.CountAsync();
        TotalPages = Math.Max(1, (int)Math.Ceiling(TotalResults / (double)pageSize));
        Products = await q.OrderBy(p => p.ProductID).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}
