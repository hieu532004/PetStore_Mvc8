using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("VoteWarehouse")]
    public class VoteWarehouse
    {
        [Key]
        public string idVotewarehouse { get; set; }

        [Required]
        public DateTime dateEntry { get; set; } = DateTime.Now;
        [ForeignKey("Supplier")]
        public string idSupplier { get; set; }


        public decimal? totalVoteWarehouse { get; set; }
        public Supplier Supplier {  get; set; }

        // Navigation Property
        public ICollection<DetailVoteWarehouse> DetailVoteWarehouses { get; set; }
        public void CalculateTotalVoteWarehouse()
        {
            if (DetailVoteWarehouses != null && DetailVoteWarehouses.Any())
            {
                totalVoteWarehouse = DetailVoteWarehouses.Sum(d => d.purchasePrice * d.Quantity);
            }
            else
            {
                totalVoteWarehouse = 0; // Nếu không có DetailOrder, đặt tổng là 0
            }
        }

    }
}
