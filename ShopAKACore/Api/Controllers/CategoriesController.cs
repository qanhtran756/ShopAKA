using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;

namespace ShopAKA.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ShopDbContext _db;

    public CategoriesController(ShopDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Category
            .Include(c => c.Product)
            .OrderBy(c => c.CategoryID)
            .Select(c => new
            {
                c.CategoryID,
                c.CategoryName,
                ProductCount = c.Product.Count
            }).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _db.Category.Include(c => c.Product).FirstOrDefaultAsync(c => c.CategoryID == id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Category category)
    {
        if (await _db.Category.AnyAsync(c => c.CategoryName.ToLower() == category.CategoryName.ToLower()))
            return BadRequest(new { message = "Danh mục đã tồn tại" });

        _db.Category.Add(category);
        await _db.SaveChangesAsync();
        return Ok(category);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Category category)
    {
        var existing = await _db.Category.FindAsync(id);
        if (existing == null) return NotFound();

        if (await _db.Category.AnyAsync(c => c.CategoryID != id && c.CategoryName.ToLower() == category.CategoryName.ToLower()))
            return BadRequest(new { message = "Danh mục đã tồn tại" });

        existing.CategoryName = category.CategoryName;
        await _db.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Category.Include(c => c.Product).FirstOrDefaultAsync(c => c.CategoryID == id);
        if (category == null) return NotFound();
        if (category.Product.Any())
            return BadRequest(new { message = $"Không thể xóa vì có {category.Product.Count} sản phẩm liên quan" });

        _db.Category.Remove(category);
        await _db.SaveChangesAsync();
        return Ok(new { success = true });
    }
}
