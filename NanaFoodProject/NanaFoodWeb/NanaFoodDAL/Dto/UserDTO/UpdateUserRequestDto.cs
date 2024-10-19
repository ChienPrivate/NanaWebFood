using NanaFoodDAL.Model;
using System.ComponentModel.DataAnnotations;


namespace NanaFoodDAL.Dto.UserDTO
{
    public class UpdateUserRequestDto
    {
        [Required(ErrorMessage = "Mã không được trống")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Tên tài khoản không được trống")]
        public string? UserName { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
                    ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, trong đó có chữ cái đầu tiên viết hoa, chứa chữ thường, số và ít nhất một ký tự đặc biệt.")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Hãy chọn vai trò cho người dùng")]
        public string Role { get; set; }
        [Required(ErrorMessage = "Email không được trống")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Xác thực Email không được rỗng")]
        public bool EmailConfirmed { get; set; } = true;
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Họ tên không được trống")]
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        [Required(ErrorMessage = "Hãy chọn trạng thái cho người dùng")]
        [EnumDataType(typeof(UserStatus))]
        public UserStatus Status { get; set; } = UserStatus.Active;
    }
}
