using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class CategoryDto
    {
        [Required]
        public string idCategory { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string nameCategory { get; set; }
    }
}
