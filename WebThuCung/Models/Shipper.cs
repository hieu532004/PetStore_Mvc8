using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Models
{
    [Table("Shipper")]
    public class Shipper
    {
        [Key]
        public string idShipper { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; }

        [Required, MaxLength(11)]
        public string Phone { get; set; }

        [Required, MaxLength(30)]
        public string userShipper { get; set; }

        [Required, MaxLength(20)]
        public string passwordShipper { get; set; }

        [Required, MaxLength(100)]
        public string Avatar { get; set; }

        [Required, MaxLength(50)]
        public string Email { get; set; }

        // Khóa ngoại tới bảng Role
        [ForeignKey("Role")]
        public string idRole { get; set; }
        public Role Role { get; set; }

        // Navigation Property: Danh sách các đơn hàng đảm nhận
        public ICollection<ShipperOrder> ShipperOrders { get; set; }
    }

}
