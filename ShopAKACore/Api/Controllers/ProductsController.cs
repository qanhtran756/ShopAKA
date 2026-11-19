using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;

namespace ShopAKA.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ShopDbContext _db;

    public ProductsController(ShopDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        var query = _db.Product.Include(p => p.Category).Where(p => p.StockQuantity > 0);
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryID == categoryId);

        var total = await query.CountAsync();
        var items = await query.OrderBy(p => p.ProductID)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => new
            {
                p.ProductID,
                p.ProductName,
                p.ProductDecription,
                p.ProductPrice,
                p.ProductImage,
                p.StockQuantity,
                p.CategoryID,
                CategoryName = p.Category!.CategoryName
            }).ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _db.Product.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductID == id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? query, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(new { total = 0, items = Array.Empty<object>() });

        var q = _db.Product.Include(p => p.Category)
            .Where(p => p.StockQuantity > 0 &&
                (p.ProductName.Contains(query) ||
                 (p.ProductDecription != null && p.ProductDecription.Contains(query))));

        var total = await q.CountAsync();
        var items = await q.OrderBy(p => p.ProductID)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => new
            {
                p.ProductID,
                p.ProductName,
                p.ProductPrice,
                p.ProductImage,
                p.StockQuantity,
                CategoryName = p.Category!.CategoryName
            }).ToListAsync();

        return Ok(new { total, page, items });
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int count = 4)
    {
        var items = await _db.Product
            .Where(p => p.StockQuantity > 0)
            .OrderByDescending(p => p.StockQuantity)
            .Take(count)
            .Select(p => new
            {
                p.ProductID,
                p.ProductName,
                p.ProductPrice,
                p.ProductImage,
                p.StockQuantity
            }).ToListAsync();
        return Ok(items);
    }

    [HttpPost("{id:int}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateRequest request)
    {
        if (HttpContext.Session.GetString("AdminUsername") == null)
            return Unauthorized(new { success = false, message = "Bạn cần đăng nhập admin" });

        var product = await _db.Product.FindAsync(id);
        if (product == null) return NotFound(new { success = false, message = "Không tìm thấy sản phẩm" });
        if (request.StockQuantity < 0)
            return BadRequest(new { success = false, message = "Số lượng tồn không được âm" });

        product.StockQuantity = request.StockQuantity;
        await _db.SaveChangesAsync();
        return Ok(new { success = true, message = "Cập nhật thành công", stockQuantity = product.StockQuantity });
    }

    public class StockUpdateRequest
    {
        public int StockQuantity { get; set; }
    }
}
