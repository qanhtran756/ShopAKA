using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopAKA.Models;
using ShopAKA.Services;

namespace ShopAKA.Pages;

public class LoginModel : PageModel
{
    private readonly AuthService _auth;

    public LoginModel(AuthService auth) => _auth = auth;

    [BindProperty]
    public LoginViewModel LoginInput { get; set; } = new();

    [BindProperty]
    public RegisterViewModel RegisterInput { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public bool ShowRegister { get; set; }

    public void OnGet(string? mode, string? returnUrl)
    {
        ReturnUrl = returnUrl;
        ShowRegister = mode == "register"
            || TempData["RegisterError"] != null
            || TempData["RegisterSuccess"] != null;
    }

    public async Task<IActionResult> OnPostLoginAsync()
    {
        ModelState.Clear();
        if (!TryValidateModel(LoginInput, nameof(LoginInput)))
        {
            TempData["LoginError"] = "Vui lòng kiểm tra lại thông tin đã nhập";
            return RedirectToPage(new { returnUrl = ReturnUrl });
        }

        var result = await _auth.LoginAsync(LoginInput.Username, LoginInput.Password);
        if (!result.Success)
        {
            TempData["LoginError"] = result.Message;
            return RedirectToPage(new { returnUrl = ReturnUrl });
        }

        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return Redirect(ReturnUrl);

        return Redirect(result.Redirect ?? "/");
    }

    public async Task<IActionResult> OnPostRegisterAsync()
    {
        ModelState.Clear();
        if (!TryValidateModel(RegisterInput, nameof(RegisterInput)))
        {
            TempData["RegisterError"] = "Vui lòng kiểm tra lại thông tin đã nhập";
            return RedirectToPage(new { mode = "register" });
        }

        var result = await _auth.RegisterAsync(RegisterInput);
        if (!result.Success)
        {
            TempData["RegisterError"] = result.Message;
            return RedirectToPage(new { mode = "register" });
        }

        TempData["RegisterSuccess"] = result.Message;
        return RedirectToPage();
    }
}
