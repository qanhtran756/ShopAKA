using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAKA.Models;

public class Customer
{
    public Customer()
    {
        Order = new HashSet<Order>();
    }

    [Key]
    public int CustomerID { get; set; }

    [Required, StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? CustomerPhone { get; set; }

    [StringLength(100)]
    public string? CustomerEmail { get; set; }

    [StringLength(200)]
    public string? CustomerAddress { get; set; }

    [StringLength(50)]
    public string? Username { get; set; }

    public virtual ICollection<Order> Order { get; set; }
    public virtual User? User { get; set; }
}
