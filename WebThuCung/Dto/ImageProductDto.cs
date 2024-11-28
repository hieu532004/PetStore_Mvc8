using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class ImageProductDto
    {
        public string idImageProduct { get; set; }
        public IFormFile? Image { get; set; }
        public string idProduct { get; set; }
    }
}
