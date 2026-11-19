using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Users;

public class IndexModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public IndexModel(ShopDbContext db) => _db = db;
    public IList<User> Users { get; set; } = new List<User>();

    public async Task<IActionResult> OnGetAsync()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Users";
        Users = await _db.User.Where(u => u.UserRole == "1").ToListAsync();
        return Page();
    }
}
