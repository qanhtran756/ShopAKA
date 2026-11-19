using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Categories;

public class DeleteModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public DeleteModel(ShopDbContext db) => _db = db;
    public Category? Category { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Categories";
        Category = await _db.Category.Include(c => c.Product).FirstOrDefaultAsync(c => c.CategoryID == id);
        return Category == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        var category = await _db.Category.Include(c => c.Product).FirstOrDefaultAsync(c => c.CategoryID == id);
        if (category == null) return NotFound();
        if (category.Product.Any())
        {
            TempData["Error"] = $"Không thể xóa vì có {category.Product.Count} sản phẩm liên quan";
            return RedirectToPage("Index");
        }
        _db.Category.Remove(category);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
