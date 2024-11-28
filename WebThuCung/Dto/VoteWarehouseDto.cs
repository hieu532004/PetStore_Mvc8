using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class VoteWarehouseDto
    {
        public string idVoteWarehouse { get; set; }

        public DateTime dateEntry { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Bạn phải chọn nhà cung cấp.")]
        public string idSupplier { get; set; }


        public decimal? totalVoteWarehouse { get; set; }
    }
}
