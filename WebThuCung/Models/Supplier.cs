using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Supplier")]
    public class Supplier
    {
        [Key]
        public string idSupplier { get; set; }

        [Required]
        [MaxLength(100)]
        public string nameSupplier { get; set; }

        [Required]
        [MaxLength(11)]
        [Phone]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        public string Email { get; set; } // EMAIL

        [MaxLength(200)]
        public string Image { get; set; } // HINHANH

        public ICollection<VoteWarehouse> VoteWarehouses { get; set; }
    }
}
