
using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class ProductDto
    {
        [Key]
        public string idProduct { get; set; }

        // Tên sản phẩm
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [MaxLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự.")]
        public string nameProduct { get; set; }

        [Required(ErrorMessage = "Giá bán là bắt buộc.")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán không được âm.")]
        public decimal sellPrice { get; set; }

        // ID chi nhánh (Branch) của sản phẩm
        [Required(ErrorMessage = "Chi nhánh là bắt buộc.")]
        public string idBranch { get; set; }

        // ID danh mục (Category) của sản phẩm
        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public string idCategory { get; set; }

        // ID thú cưng (Pet) của sản phẩm
        public string idPet { get; set; }

        // Số lượng sản phẩm
        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm.")]
        public int Quantity { get; set; }

        // Hình ảnh sản phẩm
        public IFormFile? Image { get; set; }

        // Mô tả sản phẩm
        [Required(ErrorMessage = "Mô tả sản phẩm là bắt buộc.")]
        public string Description { get; set; }
    }
}
