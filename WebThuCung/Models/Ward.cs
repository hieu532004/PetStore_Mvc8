using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Ward")]
    public class Ward
    {
        [Key]
        public string idWard { get; set; }

        [Required]
        [MaxLength(100)]
        public string nameWard { get; set; }

        // Foreign Key
        [ForeignKey("District")]
        public string idDistrict { get; set; }
        public District District { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
