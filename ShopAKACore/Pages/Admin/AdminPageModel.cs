using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShopAKA.Pages.Admin;

public abstract class AdminPageModel : PageModel
{
    protected IActionResult? RequireAdmin()
    {
        if (HttpContext.Session.GetString("AdminUsername") == null)
            return RedirectToPage("/Login");
        return null;
    }
}
