using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Transaction")]
    public class Transaction
    {
        [Key]
        public int idTransaction { get; set; }
        [Required]
        [MaxLength(100)]
        public string nameCustomer { get; set; }
        [Required]
        [MaxLength(250)]
        public string shippingAddress { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string phoneNumber { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public DateTime cpdatedDate { get; set; } = DateTime.Now;
        [MaxLength(500)]
        public string? Note { get; set; }

        // Khóa ngoại tới Customer
        [ForeignKey("Customer")]
        public int idCustomer { get; set; }
        public Customer Customer { get; set; }
        [ForeignKey("Order")]
        public string idOrder { get; set; }

        // Navigation property liên kết với Order
        public Order Order { get; set; }
	}
}
