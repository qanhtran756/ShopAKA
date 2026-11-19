using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Services;

namespace ShopAKA.Pages;

public class ProfileModel : PageModel
{
    private readonly ShopDbContext _db;
    private readonly AuthService _auth;

    public ProfileModel(ShopDbContext db, AuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    public Customer? Customer { get; set; }

    [BindProperty]
    public Customer Input { get; set; } = new();

    [BindProperty]
    public string? OldPassword { get; set; }

    [BindProperty]
    public string? NewPassword { get; set; }

    [BindProperty]
    public string? ConfirmPassword { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!_auth.IsCustomerLoggedIn) return RedirectToPage("/Login");
        Customer = await _db.Customer.Include(c => c.User).FirstOrDefaultAsync(c => c.CustomerID == _auth.CustomerId);
        if (Customer == null) return NotFound();
        Input = Customer;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!_auth.IsCustomerLoggedIn) return RedirectToPage("/Login");
        var customer = await _db.Customer.FindAsync(_auth.CustomerId);
        if (customer == null) return NotFound();

        customer.CustomerName = Input.CustomerName;
        customer.CustomerPhone = Input.CustomerPhone;
        customer.CustomerEmail = Input.CustomerEmail;
        customer.CustomerAddress = Input.CustomerAddress;
        await _db.SaveChangesAsync();
        HttpContext.Session.SetString("CustomerName", customer.CustomerName);
        TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        if (!_auth.IsCustomerLoggedIn) return RedirectToPage("/Login");

        var customer = await _db.Customer.Include(c => c.User).FirstOrDefaultAsync(c => c.CustomerID == _auth.CustomerId);
        if (customer?.User == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy thông tin tài khoản!";
            return RedirectToPage();
        }

        if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin!";
            return RedirectToPage();
        }

        if (!string.Equals(customer.User.Password?.Trim(), OldPassword.Trim(), StringComparison.Ordinal))
        {
            TempData["ErrorMessage"] = "Mật khẩu hiện tại không đúng!";
            return RedirectToPage();
        }

        if (NewPassword != ConfirmPassword)
        {
            TempData["ErrorMessage"] = "Mật khẩu mới không khớp!";
            return RedirectToPage();
        }

        if (NewPassword.Length < 6)
        {
            TempData["ErrorMessage"] = "Mật khẩu mới phải có ít nhất 6 ký tự!";
            return RedirectToPage();
        }

        customer.User.Password = NewPassword.Trim();
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
        return RedirectToPage();
    }
}
