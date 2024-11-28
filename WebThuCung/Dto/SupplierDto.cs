using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class SupplierDto
    {
        public string idSupplier { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string NameSupplier { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(11, ErrorMessage = "Phone number cannot exceed 11 characters.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }

        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }


        public IFormFile? Image { get; set; }
    }
}
