using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebThuCung.Models
{
    [Table("ShipperOrder")]
   
    public class ShipperOrder
    {

      
        [Key]
        public int idShipperOrder { get; set; }
        [ForeignKey("Order")]
        public string idOrder { get; set; }
        public Order Order { get; set; }
        [ForeignKey("Shipper")]
        public string idShipper { get; set; }
        public Shipper Shipper { get; set; }

        // Ngày nhận đơn hàng
        [Required]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        // Trạng thái vận chuyển
        [Required]
        public ShippingStatus ShippingStatus { get; set; } = ShippingStatus.Pending;
    }
    public enum ShippingStatus
    {
        Pending,       // Đang chờ xử lý
        InProgress,    // Đang vận chuyển
        Delivered,     // Đã giao hàng
        Failed         // Giao hàng thất bại
    }


}
