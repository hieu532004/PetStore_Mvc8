namespace WebThuCung.Dto
{
    public class SalesViewDto
    {
        public int TotalOrdersToday { get; set; }
        public double GrowthPercentageDay { get; set; }

        public int TotalOrdersThisWeek { get; set; }
        public double GrowthPercentageWeek { get; set; }

        public int TotalOrdersThisMonth { get; set; }
        public double GrowthPercentageMonth { get; set; }
    }
}
