using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models.Dto
{
    public class ChangePasswordDto
    {
        /// <summary>
        /// Mật khẩu hiện tại
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        /// <summary>
        /// Mật khẩu muốn thay đổi
        /// </summary>
        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm 1 chữ in hoa, 1 chữ thường và 1 số.")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Xác nhận lại mật khẩu cần thay đổi
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu nhập lại không trùng khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
