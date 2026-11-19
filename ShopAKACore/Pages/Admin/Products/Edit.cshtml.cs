using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Products;

public class EditModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    private readonly IWebHostEnvironment _env;

    public EditModel(ShopDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    [BindProperty] public Product Product { get; set; } = new();
    [BindProperty] public IFormFile? ImageFile { get; set; }
    public SelectList CategoryList { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Products";
        var product = await _db.Product.FindAsync(id);
        if (product == null) return NotFound();
        Product = product;
        CategoryList = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName", Product.CategoryID);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Products";
        var existing = await _db.Product.FindAsync(Product.ProductID);
        if (existing == null) return NotFound();
        CategoryList = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName", Product.CategoryID);
        if (!ModelState.IsValid) return Page();

        if (ImageFile != null && ImageFile.Length > 0)
        {
            var folder = Path.Combine(_env.WebRootPath, "images", "anhsanpham");
            Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid() + "_" + Path.GetFileName(ImageFile.FileName);
            await using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
            await ImageFile.CopyToAsync(stream);
            existing.ProductImage = "/images/anhsanpham/" + fileName;
        }
        existing.CategoryID = Product.CategoryID;
        existing.ProductName = Product.ProductName;
        existing.ProductDecription = Product.ProductDecription;
        existing.ProductPrice = Product.ProductPrice;
        existing.StockQuantity = Product.StockQuantity;
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
