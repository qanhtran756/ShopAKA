using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAKA.Models;

public class Order
{
    public Order()
    {
        OrderDetail = new HashSet<OrderDetail>();
    }

    [Key]
    public int OrderID { get; set; }

    public int CustomerID { get; set; }

    public DateTime OrderDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(50)]
    public string? PaymentStatus { get; set; }

    [StringLength(300)]
    public string? AddressDelivery { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual ICollection<OrderDetail> OrderDetail { get; set; }
}
