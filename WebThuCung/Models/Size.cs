using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Size")]
    public class Size
    {
        [Key]
        public string idSize { get; set; }

        public string nameSize { get; set; }
        // Navigation Property
        public ICollection<ProductSize> ProductSizess { get; set; }
    }
}
