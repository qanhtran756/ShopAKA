using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAKA.Models;

public class Category
{
    public Category()
    {
        Product = new HashSet<Product>();
    }

    [Key]
    public int CategoryID { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "Tên danh mục")]
    public string CategoryName { get; set; } = string.Empty;

    public virtual ICollection<Product> Product { get; set; }
}
