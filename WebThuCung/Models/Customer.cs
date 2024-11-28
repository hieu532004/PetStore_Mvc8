using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public int idCustomer { get; set; }

        [Required]
        [MaxLength(100)]
        public string? nameCustomer { get; set; }

        [Required]
        [MaxLength(11)]
        [Phone]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }
        [Required]
        [MaxLength(30)]
        public string userCustomer { get; set; } // TENDNKH

        [Required]
        [MaxLength(200)]
        public string passwordCustomer { get; set; } // MATKHAUKH

        [Required(ErrorMessage = "Email không được trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } // EMAIL

        public DateTime? dateBirth { get; set; } // NGAYSINH
        public DateTime createdAt { get; set; } = DateTime.Now;
        [ForeignKey("Country")]
        public string idCountry { get; set; }
        public Country Country { get; set; }

        [ForeignKey("City")]
        public string idCity { get; set; }
        public City City { get; set; }

        [ForeignKey("District")]
        public string idDistrict { get; set; }
        public District District { get; set; }

        [ForeignKey("Ward")]
        public string idWard { get; set; }
        public Ward Ward { get; set; }

        [MaxLength(200)]
        public string Image { get; set; } // HINHANH
        public bool EmailConfirmed { get; set; } = false; // Trạng thái xác nhận email
        [StringLength(6)]
        public string? OtpCode { get; set; }

        // Thời gian hết hạn của OTP
        public DateTime? OtpExpiryTime { get; set; } = DateTime.Now;

        public ICollection<SaveProduct> SaveProducts { get; set; } = new List<SaveProduct>();
        public ICollection<Order> Orders { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
