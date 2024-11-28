using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class ConfirmOtpDto
    {
        public string Email { get; set; }
        public string Otp1 { get; set; }
        public string Otp2 { get; set; }
        public string Otp3 { get; set; }
        public string Otp4 { get; set; }
        public string Otp5 { get; set; }
        public string Otp6 { get; set; }
        [Required(ErrorMessage = "Mã OTP là bắt buộc.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có 6 chữ số.")]
        public string Otp
        {
            get
            {
                // Kết hợp tất cả các ký tự OTP thành một chuỗi
                return (Otp1 ?? "") + (Otp2 ?? "") + (Otp3 ?? "") + (Otp4 ?? "") + (Otp5 ?? "") + (Otp6 ?? "");
            }
        }
    }
}
