using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Services;

namespace ShopAKA.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly ShopDbContext _db;
    private readonly AuthService _auth;
    private readonly CartService _cart;

    public OrdersController(ShopDbContext db, AuthService auth, CartService cart)
    {
        _db = db;
        _auth = auth;
        _cart = cart;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _db.Order.Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new
            {
                o.OrderID,
                o.OrderDate,
                o.TotalAmount,
                o.PaymentStatus,
                CustomerName = o.Customer!.CustomerName
            }).ToListAsync();
        return Ok(orders);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        if (!_auth.IsCustomerLoggedIn)
            return Unauthorized();

        var customerId = _auth.CustomerId!.Value;
        var orders = await _db.Order.Where(o => o.CustomerID == customerId)
            .OrderByDescending(o => o.OrderDate).ToListAsync();
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _db.Order
            .Include(o => o.Customer)
            .Include(o => o.OrderDetail).ThenInclude(d => d.Product!)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(o => o.OrderID == id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        if (request.Items == null || !request.Items.Any())
            return BadRequest(new { message = "Giỏ hàng trống" });

        decimal totalAmount = 0;
        var orderDetails = new List<OrderDetail>();

        foreach (var item in request.Items)
        {
            var product = await _db.Product.FindAsync(item.Id);
            if (product == null)
                return BadRequest(new { message = $"Sản phẩm ID {item.Id} không tồn tại" });
            if (product.StockQuantity < item.Quantity)
                return BadRequest(new { message = $"Sản phẩm {product.ProductName} không đủ tồn kho" });

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
            var existing = await _db.Customer.FirstOrDefaultAsync(c => c.CustomerPhone == request.CustomerPhone);
            if (existing != null)
            {
                customerId = existing.CustomerID;
            }
            else
            {
                var newCustomer = new Customer
                {
                    CustomerName = request.CustomerName,
                    CustomerPhone = request.CustomerPhone,
                    CustomerEmail = request.CustomerEmail,
                    CustomerAddress = request.CustomerAddress
                };
                _db.Customer.Add(newCustomer);
                await _db.SaveChangesAsync();
                customerId = newCustomer.CustomerID;
            }
        }

        var order = new Order
        {
            CustomerID = customerId,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount,
            PaymentStatus = request.PaymentMethod == "cod" ? "Chưa thanh toán" : "Đã thanh toán",
            AddressDelivery = request.CustomerAddress
        };
        _db.Order.Add(order);
        await _db.SaveChangesAsync();

        foreach (var detail in orderDetails)
        {
            detail.OrderID = order.OrderID;
            _db.OrderDetail.Add(detail);
        }
        await _db.SaveChangesAsync();
        _cart.ClearCart();

        return Ok(new { success = true, orderId = order.OrderID, totalAmount });
    }
}

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly CartService _cart;

    public CartController(CartService cart) => _cart = cart;

    [HttpGet]
    public IActionResult Get() => Ok(_cart.GetCart());

    [HttpGet("count")]
    public IActionResult Count() => Ok(new { count = _cart.GetTotalQuantity() });

    [HttpPost]
    public IActionResult Save([FromBody] List<CartItem> items)
    {
        _cart.SaveCart(items);
        return Ok(new { success = true, count = _cart.GetTotalQuantity() });
    }

    [HttpDelete]
    public IActionResult Clear()
    {
        _cart.ClearCart();
        return Ok(new { success = true });
    }
}
