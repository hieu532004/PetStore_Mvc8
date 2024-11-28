using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Color")]
    public class Color
    {
        [Key]
        public string idColor { get; set; }

        [Required]
        [MaxLength(100)]
        public string nameColor { get; set; }

        public ICollection<ProductColor> ProductColors { get; set; }
    }
}
