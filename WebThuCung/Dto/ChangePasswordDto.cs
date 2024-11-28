using System.ComponentModel.DataAnnotations;

namespace WebThuCung.Dto
{
    public class ChangePasswordDto
    {
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        [StringLength(100, ErrorMessage = "Mật khẩu mới phải có ít nhất {2} ký tự và tối đa {1} ký tự.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
