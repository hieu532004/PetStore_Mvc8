using WebThuCung.Models;

namespace WebThuCung.Dto
{
    public class SaveProductDto
    {
        public string idProduct { get; set; }
        public string nameProduct { get; set; }
        public decimal SellPrice { get; set; }
        public decimal DiscountedPrice { get; set; }  // Giá sau chiết khấu
        public int Quantity { get; set; }
        public string Image { get; set; }
    }
}
