using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Categories;

public class CreateModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public CreateModel(ShopDbContext db) => _db = db;

    [BindProperty]
    public Category Category { get; set; } = new();

    public IActionResult OnGet()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Categories";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Categories";
        if (!ModelState.IsValid) return Page();
        if (await _db.Category.AnyAsync(c => c.CategoryName.ToLower() == Category.CategoryName.ToLower()))
        {
            ModelState.AddModelError("Category.CategoryName", "Danh mục đã tồn tại");
            return Page();
        }
        _db.Category.Add(Category);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
