namespace NanaFoodWeb.Models.Dto
{
    public class UserDto
    {
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

        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public enum UserStatus
        {
            Active,
            Inactive,
            Delete,
            Block
        }
    }
    }
