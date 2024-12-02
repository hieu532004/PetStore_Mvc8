using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class AdminDto
    {
        public string? idAdmin { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string? Address { get; set; }
        [MaxLength(11)]
        public string? Phone { get; set; }
        [MaxLength(30)]
        public string? userAdmin { get; set; }
        [MaxLength(20)]
        public string? passwordAdmin { get; set; }

        public IFormFile? Avatar { get; set; }
        public string? AvatarUrl { get; set; }
        [MaxLength(50)]

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

      
    }
}
