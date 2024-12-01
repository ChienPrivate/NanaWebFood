using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models.Dto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string? UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm 1 chữ in hoa, 1 chữ thường và 1 số.")]
        [Compare(nameof(ConfirmPassword), ErrorMessage = "Mật khẩu và mật khẩu xác nhận không trùng khớp")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu và mật khẩu xác nhận không trùng khớp")]
        public string? ConfirmPassword { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập email"), EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(01|03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tên đầy đủ")]
        public string? FullName { get; set; }

    }
}
