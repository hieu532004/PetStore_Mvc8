namespace WebThuCung.Dto
{
    public class DashboardViewDto
    {
        public SalesViewDto Sales { get; set; }
        public RevenueViewDto Revenue { get; set; }
        public CustomerViewDto Customer { get; set; }
        public AdminDto Admin { get; set; }
        public List<TopSellingProductDto> TopSellingProducts { get; set; }
    }
}
