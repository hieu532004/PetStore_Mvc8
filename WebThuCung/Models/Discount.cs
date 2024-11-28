using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Discount")]
    public class Discount
    {
        [Key]
        public string idDiscount { get; set; }

        [Required]
        public string  idProduct { get; set; }

        [Required]
        public int discountPercent { get; set; }
        // Navigation Property
        [ForeignKey("idProduct")]
        public Product Product { get; set; }
    }
}
