using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Warehouse;

public class IndexModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    public IndexModel(ShopDbContext db) => _db = db;
    public List<Product> InStock { get; set; } = new();
    public List<Product> OutOfStock { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Warehouse";
        var all = await _db.Product.Include(p => p.Category).ToListAsync();
        InStock = all.Where(p => p.StockQuantity > 0).ToList();
        OutOfStock = all.Where(p => p.StockQuantity == 0).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStockAsync(int productId, int stockQuantity)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        var product = await _db.Product.FindAsync(productId);
        if (product == null) { TempData["Error"] = "Không tìm thấy sản phẩm"; return RedirectToPage(); }
        if (stockQuantity < 0) { TempData["Error"] = "Số lượng không hợp lệ"; return RedirectToPage(); }
        product.StockQuantity = stockQuantity;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Cập nhật tồn kho thành công";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateStockAjaxAsync(int productId, int stockQuantity)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return new JsonResult(new { success = false, message = "Unauthorized" });

        try
        {
            var product = await _db.Product.FindAsync(productId);
            if (product == null) return new JsonResult(new { success = false, message = "Không tìm thấy sản phẩm" });
            if (stockQuantity < 0) return new JsonResult(new { success = false, message = "Số lượng tồn không được âm" });

            product.StockQuantity = stockQuantity;
            await _db.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Cập nhật số lượng tồn thành công", stockQuantity });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }
}
