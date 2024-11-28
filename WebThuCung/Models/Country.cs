using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Country")]
    public class Country
    {
        [Key]
        public string idCountry { get; set; }

        [Required]
        [MaxLength(100)]
        public string nameCountry { get; set; }

        // Navigation Property
        public ICollection<City> Cities { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }

}
