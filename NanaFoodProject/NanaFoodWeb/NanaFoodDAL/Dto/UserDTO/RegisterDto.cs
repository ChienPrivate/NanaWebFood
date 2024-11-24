using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto.UserDTO
{
    public class RegisterDto
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        public string? UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm 1 chữ in hoa, 1 chữ thường và 1 số.")]
        public string? Password { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required, EmailAddress(ErrorMessage = "Email không hợp lệ)")]
        public string? Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tên đầy đủ")]
        public string? FullName { get; set; }
    }
}
