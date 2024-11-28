using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class BranchCreateDto
    {
        [Required(ErrorMessage = "Branch ID is required.")]
        [StringLength(50, ErrorMessage = "Branch ID cannot exceed 50 characters.")]
        public string idBranch { get; set; }

        [Required(ErrorMessage = "Branch Name is required.")]
        [StringLength(100, ErrorMessage = "Branch Name cannot exceed 100 characters.")]
        public string nameBranch { get; set; }

        public IFormFile Logo { get; set; }
    }
}
