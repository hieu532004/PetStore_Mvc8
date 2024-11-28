using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Admin")]
    public class Admin
    {
        [Key]
        public string idAdmin { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; }

        [Required, MaxLength(11)]
        public string Phone { get; set; }

        [Required, MaxLength(30)]
        public string userAdmin { get; set; }

        [Required, MaxLength(20)]
        public string passwordAdmin { get; set; }

        [Required, MaxLength(100)]
        public string Avatar { get; set; }

        [Required, MaxLength(50)]
        public string Email { get; set; }

        // Navigation Property
        [ForeignKey("Role")]
        public string idRole { get; set; }
        public Role Role { get; set; }
    }
}
