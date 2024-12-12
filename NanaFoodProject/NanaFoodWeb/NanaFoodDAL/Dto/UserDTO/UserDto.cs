using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto.UserDTO
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
