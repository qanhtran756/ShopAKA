using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;

namespace ShopAKA.ViewComponents;

public class CategoryMenuViewComponent : ViewComponent
{
    private readonly ShopDbContext _db;

    public CategoryMenuViewComponent(ShopDbContext db) => _db = db;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _db.Category.OrderBy(c => c.CategoryID).ToListAsync();
        return View(categories);
    }
}

public class CartBadgeViewComponent : ViewComponent
{
    private readonly Services.CartService _cart;

    public CartBadgeViewComponent(Services.CartService cart) => _cart = cart;

    public IViewComponentResult Invoke()
    {
        return View(_cart.GetTotalQuantity());
    }
}

public class AdminSidebarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string? activePage = null)
    {
        return View("Default", activePage);
    }
}

public class FeaturedProductsViewComponent : ViewComponent
{
    private readonly ShopDbContext _db;

    public FeaturedProductsViewComponent(ShopDbContext db) => _db = db;

    public async Task<IViewComponentResult> InvokeAsync(int count = 4)
    {
        var products = await _db.Product
            .Where(p => p.StockQuantity > 0)
            .OrderByDescending(p => p.StockQuantity)
            .Take(count)
            .ToListAsync();
        return View(products);
    }
}
