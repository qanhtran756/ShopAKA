using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Categories;

public class IndexModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public IndexModel(ShopDbContext db) => _db = db;
    public IList<Category> Categories { get; set; } = new List<Category>();

    public async Task<IActionResult> OnGetAsync()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Categories";
        Categories = await _db.Category.Include(c => c.Product).OrderBy(c => c.CategoryID).ToListAsync();
        return Page();
    }
}
