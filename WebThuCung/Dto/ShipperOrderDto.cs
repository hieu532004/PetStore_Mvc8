namespace WebThuCung.Dto
{
    public class ShipperOrderDto
    {
        public int idShipperOrder { get; set; }  // ID của đơn hàng

        public string idOrder { get; set; }  // ID của đơn hàng
        public string nameCustomer { get; set; }  // Tên khách hàng
        public DateTime OrderDate { get; set; }  // Ngày tạo đơn hàng
        public string ShippingStatus { get; set; }  // Trạng thái vận chuyển
        public string ShipperId { get; set; }  // ID của shipper
        public string ShipperName { get; set; }  // Tên của shipper
        public DateTime AssignedDate { get; set; }
    }
}
