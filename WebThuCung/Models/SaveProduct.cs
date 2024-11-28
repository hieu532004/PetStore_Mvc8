using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebThuCung.Models
{
    [Table("SaveProduct")]
    [PrimaryKey("idProduct", "idCustomer")]
    public class SaveProduct
    {
        [Key]
        [Column(Order = 1)]
        public string idProduct { get; set; }

        [Key]
        [Column(Order = 2)]
        public int idCustomer { get; set; }

        public Customer Customer { get; set; }
        public Product Product { get; set; }

        [Required]
        public DateTime SavedAt { get; set; } = DateTime.Now;
    }
}
