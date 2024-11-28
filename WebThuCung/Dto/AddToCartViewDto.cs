namespace WebThuCung.Dto
{
    public class AddToCartViewDto
    {
        public string ProductId { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int Quantity { get; set; }
    }
}
