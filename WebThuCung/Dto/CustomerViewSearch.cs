using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class CustomerViewSearch
    {
        public int idCustomer { get; set; }

        [Required]
        [MaxLength(100)]
        public string nameCustomer { get; set; }

        [Required]
        [MaxLength(11)]
        [Phone]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [MaxLength(30)]
        public string userCustomer { get; set; } // Username

        [Required]
        [MaxLength(200)]
        public string passwordCustomer { get; set; } // Password

        [Required(ErrorMessage = "Email không được trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } // Email

        public DateTime? dateBirth { get; set; } // Date of Birth

        public DateTime createdAt { get; set; } = DateTime.Now;
        [MaxLength(200)]
        public string Image { get; set; } // Image URL
    }
}
