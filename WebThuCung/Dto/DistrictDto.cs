using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class DistrictDto
    {
        [Key]
        public string idDistrict { get; set; }

        [Required]
        [MaxLength]
        public string nameDistrict { get; set; }

        [Required]
        public string idCity { get; set; } // ID của thành phố mà quận thuộc về
    }
}
