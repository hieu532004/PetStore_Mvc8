namespace WebThuCung.Dto
{
    public class ImageProductCreateDto
    {
        public string idImageProduct { get; set; }
        public IFormFile Image { get; set; }
        public string idProduct { get; set; }
    }
}
