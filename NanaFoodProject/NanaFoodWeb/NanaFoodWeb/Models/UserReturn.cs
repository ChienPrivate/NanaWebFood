namespace NanaFoodWeb.Models
{
    public class UserReturn
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Địa chỉ email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Jwt Token
        /// </summary>
        public string Token { get; set; }
    }
}
