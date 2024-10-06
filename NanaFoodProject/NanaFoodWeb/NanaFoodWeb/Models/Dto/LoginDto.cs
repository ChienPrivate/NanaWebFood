using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models.Dto
{
    public class LoginDto
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        public string Password { get; set; }

        /// <summary>
        /// Giữ trạng thái đăng nhập
        /// </summary>
        public bool keepLogined { get; set; } = false;
    }
}
