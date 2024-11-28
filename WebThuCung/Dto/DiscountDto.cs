using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class DiscountDto
    {
        public string idDiscount { get; set; }

        // Mã định danh cho sản phẩm (Product)
        public string idProduct { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Discount cannot be negative.")]
        public int discountPercent { get; set; }
    }
}
