using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Services;

namespace ShopAKA.Pages;

public class CheckoutModel : PageModel
{
    private readonly ShopDbContext _db;
    private readonly AuthService _auth;

    public CheckoutModel(ShopDbContext db, AuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    public Customer? Customer { get; set; }

    [BindProperty]
    public CheckoutForm Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (_auth.IsCustomerLoggedIn)
        {
            Customer = await _db.Customer.FindAsync(_auth.CustomerId);
            if (Customer != null)
            {
                Input.CustomerName = Customer.CustomerName;
                Input.CustomerPhone = Customer.CustomerPhone ?? "";
                Input.CustomerEmail = Customer.CustomerEmail ?? "";
                Input.CustomerAddress = Customer.CustomerAddress ?? "";
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? orderItems)
    {
        if (string.IsNullOrEmpty(orderItems))
        {
            TempData["CheckoutError"] = "Giỏ hàng trống!";
            return RedirectToPage("/Cart");
        }

        List<CheckoutItemJson>? items;
        try
        {
            items = JsonSerializer.Deserialize<List<CheckoutItemJson>>(orderItems,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            TempData["CheckoutError"] = "Dữ liệu giỏ hàng không hợp lệ!";
            return RedirectToPage("/Cart");
        }

        if (items == null || items.Count == 0)
        {
            TempData["CheckoutError"] = "Giỏ hàng trống!";
            return RedirectToPage("/Cart");
        }

        decimal totalAmount = 0;
        var orderDetails = new List<OrderDetail>();

        foreach (var item in items)
        {
            var product = await _db.Product.FindAsync(item.Id);
            if (product == null)
            {
                TempData["CheckoutError"] = $"Sản phẩm với ID {item.Id} không tồn tại!";
                return RedirectToPage("/Cart");
            }
            if (product.StockQuantity < item.Quantity)
            {
                TempData["CheckoutError"] = $"Sản phẩm {product.ProductName} không đủ số lượng tồn kho!";
                return RedirectToPage("/Cart");
            }

            totalAmount += item.Price * item.Quantity;
            orderDetails.Add(new OrderDetail
            {
                ProductID = item.Id,
                Quantity = item.Quantity,
                UnitPrice = item.Price
            });
            product.StockQuantity -= item.Quantity;
        }

        int customerId;
        if (_auth.IsCustomerLoggedIn)
        {
            customerId = _auth.CustomerId!.Value;
        }
        else
        {
            var existing = await _db.Customer.FirstOrDefaultAsync(c => c.CustomerPhone == Input.CustomerPhone);
            if (existing != null)
            {
                customerId = existing.CustomerID;
            }
            else
            {
                var nc = new Customer
                {
                    CustomerName = Input.CustomerName,
                    CustomerPhone = Input.CustomerPhone,
                    CustomerEmail = Input.CustomerEmail,
                    CustomerAddress = Input.CustomerAddress
                };
                _db.Customer.Add(nc);
                await _db.SaveChangesAsync();
                customerId = nc.CustomerID;
            }
        }

        var order = new Order
        {
            CustomerID = customerId,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount,
            PaymentStatus = Input.PaymentMethod == "cod" ? "Chưa thanh toán" : "Đã thanh toán",
            AddressDelivery = Input.CustomerAddress
        };
        _db.Order.Add(order);
        await _db.SaveChangesAsync();
        foreach (var d in orderDetails)
        {
            d.OrderID = order.OrderID;
            _db.OrderDetail.Add(d);
        }
        await _db.SaveChangesAsync();

        TempData["PaymentMethod"] = Input.PaymentMethod == "cod"
            ? "Thanh toán khi nhận hàng (COD)"
            : "Thẻ tín dụng/ghi nợ";
        return RedirectToPage("/CheckoutSuccess", new { orderId = order.OrderID });
    }

    public class CheckoutForm
    {
        [Required] public string CustomerName { get; set; } = "";
        [Required] public string CustomerPhone { get; set; } = "";
        [EmailAddress] public string? CustomerEmail { get; set; }
        [Required] public string CustomerAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "cod";
        public string? Note { get; set; }
    }

    private class CheckoutItemJson
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
