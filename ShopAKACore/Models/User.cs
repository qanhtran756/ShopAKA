using System.ComponentModel.DataAnnotations;

namespace ShopAKA.Models;

public class User
{
    public User()
    {
        Customer = new HashSet<Customer>();
    }

    [Key, StringLength(50)]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Mật khẩu")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [StringLength(10)]
    public string UserRole { get; set; } = "2";

    public virtual ICollection<Customer> Customer { get; set; }
}
