using NanaFoodDAL.Model;

namespace NanaFoodDAL.Dto.UserDTO
{
    public class UserWithRolesDto
    {
        public string Id { get; set; }           // Mã người dùng
        public string AvatarUrl { get; set; }    // Đường dẫn hình ảnh đại diện
        public string FullName { get; set; }     // Họ tên
        public string UserName { get; set; }     // Tên đăng nhập
        public UserStatus Status { get; set; }   // Trạng thái (Có thể dùng enum nếu cần)
        public string PhoneNumber { get; set; }  // Số điện thoại
        public string Address { get; set; }      // Địa chỉ
        public string Email { get; set; }        // Địa chỉ email
        public bool EmailConfirmed { get; set; } // Kích hoạt Email
        public string Roles { get; set; }        // Vai trò người dùng
    }
}
