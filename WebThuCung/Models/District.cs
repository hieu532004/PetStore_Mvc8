using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("District")]
    public class District
    {
        [Key]
        public string idDistrict { get; set; }

        [Required]
        [MaxLength(100)]
        public string nameDistrict { get; set; }

        // Foreign Key
        [ForeignKey("City")]
        public string idCity { get; set; }
        public City City { get; set; }

        // Navigation Property
        public ICollection<Ward> Wards { get; set; }
        public ICollection<Customer> Customers { get; set; }

    }
}
