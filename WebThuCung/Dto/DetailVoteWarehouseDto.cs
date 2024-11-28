using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class DetailVoteWarehouseDto
    {
        public string idVoteWarehouse { get; set; }

        [Required(ErrorMessage = "You must select a product.")]
        public string idProduct { get; set; }

        [Required(ErrorMessage = "Quantity cannot be left blank.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Purchase price cannot be left blank.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Purchase price must be greater than 0.")]
        public decimal purchasePrice { get; set; }

        public decimal? totalPrice { get; set; }
    }
}
