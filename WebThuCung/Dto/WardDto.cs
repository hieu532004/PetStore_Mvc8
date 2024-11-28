using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class WardDto
    {
        public string idWard { get; set; }

        [Required(ErrorMessage = "Ward name is required.")]
        [MaxLength(100, ErrorMessage = "Ward name cannot exceed 100 characters.")]
        public string nameWard { get; set; }

        [Required(ErrorMessage = "District ID is required.")]
        public string idDistrict { get; set; }
    }
}
