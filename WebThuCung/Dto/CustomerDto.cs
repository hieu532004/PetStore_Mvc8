using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebThuCung.Models;

namespace WebThuCung.Dto
{
    public class CustomerDto
    {
        [MaxLength(100)]
        public string? nameCustomer { get; set; }

        [MaxLength(11)]
        [Phone]
        public string? Phone { get; set; }

        [MaxLength(100)]

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        public DateTime? dateBirth { get; set; }
        public string? idCountry { get; set; }
        public Country? Country { get; set; }

        public string? idCity { get; set; }
        public City? City { get; set; }

        public string? idDistrict { get; set; }
        public District? District { get; set; }

        public string? idWard { get; set; }
        public Ward? Ward { get; set; }
        public IFormFile? Image { get; set; }
    }
}
