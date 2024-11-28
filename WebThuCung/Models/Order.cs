using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public string idOrder { get; set; }

        [ForeignKey("Customer")]
        public int idCustomer { get; set; }

        [Required]
        public DateTime dateFrom { get; set; } = DateTime.Now;

        public DateTime? dateTo { get; set; }

        public OrderStatus? statusOrder { get; set; } = OrderStatus.Pending;

        public PaymentStatus? statusPay { get; set; } = PaymentStatus.Unpaid;

        public decimal? totalOrder { get; set; }

        // Navigation Properties
        public Customer Customer { get; set; }
        public ICollection<DetailOrder> DetailOrders { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<ShipperOrder> ShipperOrders { get; set; }
        public void CalculateTotalOrder()
        {
            if (DetailOrders != null && DetailOrders.Any())
            {
                totalOrder = DetailOrders.Sum(d => d.Quantity * (d.Product != null ? d.Product.sellPrice : 0));
            }
            else
            {
                totalOrder = 0; // Nếu không có DetailOrder, đặt tổng là 0
            }
        }

    }
    public enum OrderStatus
    {
        InCart,
        Pending,
        Accepted,
        Rejected,// Đang chờ xử lý
        Complete  // Đã từ chối
    }
    public enum PaymentStatus
    {
        Unpaid,   // Chưa thanh toán
        Paid,     // Đã thanh toán
        Refunded  // Đã hoàn tiền
    }

}
