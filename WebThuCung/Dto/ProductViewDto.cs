namespace WebThuCung.Dto
{
    public class ProductViewDto
    {
        public string idProduct { get; set; }
        public string nameProduct { get; set; }
        public decimal sellPrice { get; set; }
        public string Image { get; set; }
        public string idBranch { get; set; }
        public string idCategori { get; set; }
        public string idPet { get; set; }
        public string nameBranch { get; set; }
        public string nameCategory { get; set; }
        public string namePet { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public List<string> Colors{ get; set; }
        public List<string> Sizes { get; set; }
        public List<string>  ImageProducts {  get; set; }
        public decimal DiscountedPrice { get; set; }
        public int DiscountPercent { get; set; }

        public string Logo { get; set; }
        public List<int> Discounts { get; set; }
        public List<TopSellingProductDto> TopSellingProducts { get; set; }

    }
}
