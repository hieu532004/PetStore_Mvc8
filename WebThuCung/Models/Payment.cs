using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCung.Models
{
	[Table("Payment")]
	public class Payment
	{
		[Key]
		public int Id { get; set; }
		public int idTransaction { get; set; }
		public decimal Amount { get; set; }
		public DateTime CreatedDate { get; set; }
		public string QRCodeUrl { get; set; }
	}


}
