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
        [Required]
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; } = "https://placehold.co/300x300";
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
