using Microsoft.AspNetCore.Http;
using ShopAKA.Data;
using ShopAKA.Models;
using Microsoft.EntityFrameworkCore;

namespace ShopAKA.Services;

public class AuthService
{
    private readonly ShopDbContext _db;
    private readonly IHttpContextAccessor _http;

    public AuthService(ShopDbContext db, IHttpContextAccessor http)
    {
        _db = db;
        _http = http;
    }

    private ISession Session => _http.HttpContext!.Session;

    public bool IsCustomerLoggedIn => Session.GetInt32("CustomerID") != null;
    public bool IsAdminLoggedIn => Session.GetString("AdminUsername") != null;
    public int? CustomerId => Session.GetInt32("CustomerID");
    public string? CustomerName => Session.GetString("CustomerName");

    public async Task<(bool Success, string Message, string? Redirect)> LoginAsync(string username, string password)
    {
        var user = await _db.User.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        if (user == null)
            return (false, "Tên đăng nhập hoặc mật khẩu không đúng", null);

        if (user.UserRole == "1")
        {
            Session.SetString("AdminUsername", user.Username);
            Session.SetString("AdminRole", user.UserRole);
            return (true, "Đăng nhập admin thành công", "/Admin/Categories/Index");
        }

        Session.SetString("Username", user.Username);
        Session.SetString("UserRole", user.UserRole);
        var customer = await _db.Customer.FirstOrDefaultAsync(c => c.Username == user.Username);
        if (customer != null)
        {
            Session.SetInt32("CustomerID", customer.CustomerID);
            Session.SetString("CustomerName", customer.CustomerName);
        }

        return (true, "Đăng nhập thành công", "/");
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model)
    {
        if (await _db.User.AnyAsync(u => u.Username == model.Username))
            return (false, "Tên đăng nhập đã tồn tại");

        if (await _db.Customer.AnyAsync(c => c.CustomerEmail == model.CustomerEmail))
            return (false, "Email đã được sử dụng");

        if (await _db.Customer.AnyAsync(c => c.CustomerPhone == model.CustomerPhone))
            return (false, "Số điện thoại đã được sử dụng");

        var user = new User { Username = model.Username, Password = model.Password, UserRole = "2" };
        _db.User.Add(user);
        await _db.SaveChangesAsync();

        _db.Customer.Add(new Customer
        {
            CustomerName = model.CustomerName,
            CustomerPhone = model.CustomerPhone,
            CustomerEmail = model.CustomerEmail,
            CustomerAddress = model.CustomerAddress ?? "",
            Username = model.Username
        });
        await _db.SaveChangesAsync();
        return (true, "Đăng ký thành công! Vui lòng đăng nhập");
    }

    public void Logout()
    {
        Session.Clear();
    }

    public void LogoutAdmin()
    {
        Session.Remove("AdminUsername");
        Session.Remove("AdminRole");
    }
}
