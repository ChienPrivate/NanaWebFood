using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models.Dto
{
    public class UserDto
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        // public string UserId { get; set; }

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Địa chỉ email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Đường dẫn ảnh đại diện
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Đường dẫn ảnh đại diện
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Đường dẫn ảnh đại diện
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [Required]
        [EnumDataType(typeof(UserStatus))]
        public UserStatus Status { get; set; }
        
    }
/*    public enum UserStatus
    {
        Active,
        Inactive,
        Delete,
        Block
    }*/
}
