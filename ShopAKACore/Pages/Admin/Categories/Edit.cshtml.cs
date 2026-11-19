using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Categories;

public class EditModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public EditModel(ShopDbContext db) => _db = db;

    [BindProperty]
    public Category Category { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Categories";
        var cat = await _db.Category.FindAsync(id);
        if (cat == null) return NotFound();
        Category = cat;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Categories";
        if (!ModelState.IsValid) return Page();
        var existing = await _db.Category.FindAsync(Category.CategoryID);
        if (existing == null) return NotFound();
        if (await _db.Category.AnyAsync(c => c.CategoryID != Category.CategoryID && c.CategoryName.ToLower() == Category.CategoryName.ToLower()))
        {
            ModelState.AddModelError("Category.CategoryName", "Danh mục đã tồn tại");
            return Page();
        }
        existing.CategoryName = Category.CategoryName;
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
