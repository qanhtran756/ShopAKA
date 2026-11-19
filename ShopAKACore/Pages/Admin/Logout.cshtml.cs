using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopAKA.Services;

namespace ShopAKA.Pages.Admin;

public class LogoutModel : AdminPageModel
{
    private readonly AuthService _auth;
    public LogoutModel(AuthService auth) => _auth = auth;
    public IActionResult OnGet()
    {
        _auth.LogoutAdmin();
        return RedirectToPage("/Index");
    }
}
