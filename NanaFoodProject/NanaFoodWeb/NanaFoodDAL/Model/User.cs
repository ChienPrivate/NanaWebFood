using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Model
{
    public class User : IdentityUser
    {
        public string? Avatar { get; set; }
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Độ dài của tên phải từ 3 đến 50 ký tự")]
        public string FullName { get; set; }
        [Range(1,3)]
        public int Gender { get; set; }
        [Required]
        [EnumDataType(typeof(UserStatus))]
        public UserStatus Status { get; set; } = UserStatus.Active;
        public List<Review> Reviews { get; set; }
        public List<WishList> WishLists { get; set; }
        public List<Order> Orders { get; set; }
        public List<SearchHistory> SearchHistories { get; set; }
        public Cart Cart { get; set; }
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Delete,
        Block
    }
}
