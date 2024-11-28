using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebThuCung.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public string idRole { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public virtual ICollection<Admin> Admins { get; set; } // Link với Admin
    }
}
