namespace WebThuCung.Dto
{
    public class TopSellingProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Sold { get; set; }
        public decimal Revenue { get; set; }
        public string ImageUrl { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int DiscountPercent {  get; set; }
    }
}
