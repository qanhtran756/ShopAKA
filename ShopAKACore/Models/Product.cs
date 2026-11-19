using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAKA.Models;

public class Product
{
    public Product()
    {
        OrderDetail = new HashSet<OrderDetail>();
    }

    [Key]
    public int ProductID { get; set; }

    public int CategoryID { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "Tên sản phẩm")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    public string? ProductDecription { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá")]
    public decimal ProductPrice { get; set; }

    [Display(Name = "Ảnh")]
    public string? ProductImage { get; set; }

    [Display(Name = "Tồn kho")]
    public int StockQuantity { get; set; }

    public virtual Category? Category { get; set; }
    public virtual ICollection<OrderDetail> OrderDetail { get; set; }
}
