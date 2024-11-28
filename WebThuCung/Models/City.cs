using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("City")]
    public class City
    {
        [Key]
        public string idCity { get; set; }

        [Required]
        [MaxLength(100)]
        public string nameCity { get; set; }

        // Foreign Key
        [ForeignKey("Country")]
        public string idCountry { get; set; }
        public Country Country { get; set; }

        // Navigation Property
        public ICollection<District> Districts { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
