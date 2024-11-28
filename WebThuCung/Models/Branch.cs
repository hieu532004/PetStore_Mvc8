using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Branch")]
    public class Branch
    {
        [Key]
        public string idBranch { get; set; }

        [Required, MaxLength(100)]
        public string nameBranch { get; set; }

        [MaxLength(100)]
        public string Logo { get; set; }

        // Navigation Property
        public ICollection<Product> Products { get; set; }
    }
}
