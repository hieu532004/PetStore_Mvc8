using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public string idCategory { get; set; }

        [Required]
        [MaxLength(200)]
        public string nameCategory { get; set; }

        // Navigation Property
        public ICollection<Product> Products { get; set; }
    }
}
