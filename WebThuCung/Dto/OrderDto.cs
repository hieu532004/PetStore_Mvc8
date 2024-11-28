using System.ComponentModel.DataAnnotations;
using WebThuCung.Models;

namespace WebThuCung.Dto
{
    public class OrderDto
    {
        [Key]
        public string idOrder { get; set; }

        [Required]
        public int idCustomer { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime dateFrom { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? dateTo { get; set; }

        [Required]
        public OrderStatus? statusOrder { get; set; }

        public PaymentStatus? statusPay { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total order must be a positive number.")]
        public decimal? totalOrder { get; set; }
    }
}
