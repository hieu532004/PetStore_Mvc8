using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("ProductSIze")]
    [PrimaryKey("idSize", "idProduct")]
    public class ProductSize
    {
      
        [Key, Column(Order = 0)]
        public string idSize { get; set; }
        public Size Size { get; set; }
        [Key, Column(Order = 1)]
        public string idProduct { get; set; }
        public Product Product { get; set; }
    }
}
