using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class CountryDto
    {
        [Required]
        public string idCountry { get; set; }

        [Required]
        [MaxLength(100)]
        public string nameCountry { get; set; }
    }

}
