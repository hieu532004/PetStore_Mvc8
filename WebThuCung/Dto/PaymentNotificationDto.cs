namespace WebThuCung.Dto
{
    public class PaymentNotificationDto
    {
        public int TransactionId { get; set; } // ID của giao dịch
        public bool IsSuccess { get; set; } // Trạng thái thanh toán (thành công hay không)
        public string Message { get; set; }
    }
}
