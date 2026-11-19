using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Users;

public class CreateModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public CreateModel(ShopDbContext db) => _db = db;

    [BindProperty] public User AdminUser { get; set; } = new();

    public IActionResult OnGet()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Users";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Users";
        if (!ModelState.IsValid) return Page();
        if (await _db.User.AnyAsync(u => u.Username == AdminUser.Username))
        {
            ModelState.AddModelError("AdminUser.Username", "Tên đăng nhập đã tồn tại");
            return Page();
        }
        AdminUser.UserRole = "1";
        _db.User.Add(AdminUser);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
