using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("ProductColor")]
    [PrimaryKey("idColor", "idProduct")]
    public class ProductColor
    {
 
        [Key, Column(Order = 0)]
        public string idColor { get; set; }
        public Color Color { get; set; }
        [Key, Column(Order = 1)]
        public string idProduct { get; set; }
        public Product Product { get; set; }
    }
}
