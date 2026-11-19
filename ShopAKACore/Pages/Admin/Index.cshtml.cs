using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShopAKA.Pages.Admin;

public class IndexModel : AdminPageModel
{
    public IActionResult OnGet()
    {
        var redirect = RequireAdmin();
        return redirect ?? RedirectToPage("/Admin/Categories/Index");
    }
}
