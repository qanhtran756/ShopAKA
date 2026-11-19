using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopAKA.Data;
using ShopAKA.Models;
using ShopAKA.Pages.Admin;

namespace ShopAKA.Pages.Admin.Products;

public class CreateModel : AdminPageModel
{
    private readonly ShopDbContext _db;
    private readonly IWebHostEnvironment _env;

    public CreateModel(ShopDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    [BindProperty] public Product Product { get; set; } = new();
    [BindProperty] public IFormFile? ImageFile { get; set; }
    public SelectList CategoryList { get; set; } = null!;

    public IActionResult OnGet()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Products";
        CategoryList = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        ViewData["ActivePage"] = "Products";
        CategoryList = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName", Product.CategoryID);
        if (!ModelState.IsValid) return Page();
        if (ImageFile != null && ImageFile.Length > 0)
        {
            var folder = Path.Combine(_env.WebRootPath, "images", "anhsanpham");
            Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid() + "_" + Path.GetFileName(ImageFile.FileName);
            await using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
            await ImageFile.CopyToAsync(stream);
            Product.ProductImage = "/images/anhsanpham/" + fileName;
        }
        _db.Product.Add(Product);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
