using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebThuCung.Models
{
    [Table("DetailVoteWarehouse")]
    [PrimaryKey("idVotewarehouse", "idProduct")]
    public class DetailVoteWarehouse
    {
        [Key, Column(Order = 0)]
        public string idVotewarehouse { get; set; }

        [Key, Column(Order = 1)]
        public string idProduct { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal purchasePrice { get; set; }

        public decimal? totalPrice { get; set; }

        // Navigation Properties
        public VoteWarehouse VoteWarehouse { get; set; }
        public Product Product { get; set; }
        public decimal CalculateTotalPriceWare()
        {
            return Quantity * purchasePrice; // Tính toán tổng giá
        }
    }
}
