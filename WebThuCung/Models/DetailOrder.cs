using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("DetailOrder")]
    public class DetailOrder
    {
        [Key] // Đặt một khóa chính cho bảng này
        public int IdDetailOrder { get; set; }
        [ForeignKey("Order")]
        public string idOrder { get; set; }

        [ForeignKey("Product")]
        public string idProduct { get; set; }

        [Required]
        public int Quantity { get; set; }
        public string nameSize { get; set; }
        public string nameColor { get; set; }
        public decimal? totalPrice { get; set; }

        // Navigation Properties
        public Order Order { get; set; }
        public Product Product { get; set; }
        public decimal CalculateTotalPrice()
        {
            // Kiểm tra nếu Product không bị null
            if (Product != null)
            {
                // Kiểm tra nếu Product có giảm giá và giá trị giảm giá không bị null
                var discountPercent = Product.Discounts?.FirstOrDefault()?.discountPercent ?? 0;

                // Tính toán giá sau khi áp dụng giảm giá (nếu có)
                var discountedPrice = Product.sellPrice - (Product.sellPrice * discountPercent / 100);

                return Quantity * discountedPrice;
            }

            return 0; // Trả về 0 nếu Product là null
        }


    }

}
