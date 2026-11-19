using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShopAKA.Pages;

public class RegisterModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/Login", new { mode = "register" });
}
