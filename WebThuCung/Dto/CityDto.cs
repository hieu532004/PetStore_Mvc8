using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class CityDto
    {
        [Required]
        [MaxLength(50)] // Adjust as necessary for ID length constraints
        public string idCity { get; set; }

        [Required]
        [MaxLength(100)]
        public string nameCity { get; set; }

        [Required]
        public string idCountry { get; set; }
    }
}
