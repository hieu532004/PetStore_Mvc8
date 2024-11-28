using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Pet")]
    public class Pet
    {
        [Key]
        public string idPet { get; set; }

        [Required]
        [MaxLength(200)]
        public string namePet { get; set; }

        // Navigation Property
        public ICollection<Product> Products { get; set; }
    }
}
