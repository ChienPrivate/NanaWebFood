﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto.UserDTO
{
    public class UserDto
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }

        public enum UserStatus
        {
            Active,
            Inactive,
            Delete,
            Block
        }
    }
}
