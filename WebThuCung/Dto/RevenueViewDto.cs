namespace WebThuCung.Dto
{
    public class RevenueViewDto
    {
        public decimal TotalRevenueToday { get; set; }
        public decimal TotalRevenueThisMonth { get; set; }
        public decimal TotalRevenueThisWeek { get; set; }

        public double GrowthPercentageDay { get; set; }
        public double GrowthPercentageMonth { get; set; }
        public double GrowthPercentageWeek { get; set; }
    }
}
