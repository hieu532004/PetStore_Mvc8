using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class ColorDto
    {
        [Required]
        public string idColor { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string nameColor { get; set; }
    }
}
