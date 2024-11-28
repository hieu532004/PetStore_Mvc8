namespace WebThuCung.Dto
{
    public class CustomerViewDto
    {
        public decimal TotalCustomerToday { get; set; }
        public decimal TotalCustomerThisMonth { get; set; }
        public decimal TotalCustomerThisWeek { get; set; }

        public double GrowthPercentageDay { get; set; }
        public double GrowthPercentageMonth { get; set; }
        public double GrowthPercentageWeek { get; set; }
    }
}
