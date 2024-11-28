using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class PetDto
    {
        [Required]
        public string idPet { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string namePet { get; set; }
    }
}
