using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class LoginDto
    {
        [Display(Name = "Tên đăng nhập:")]
        [Required(ErrorMessage = " Tên đăng nhập không được để trống. ")]
        public string userName { set; get; }

        [Display(Name = "Mật khẩu:")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Độ dài mật khẩu ít nhất 6 - 20 ký tự. ")]
        [Required(ErrorMessage = "Mật khẩu không được để trống. ")]
        public string password { set; get; }
    }
}
