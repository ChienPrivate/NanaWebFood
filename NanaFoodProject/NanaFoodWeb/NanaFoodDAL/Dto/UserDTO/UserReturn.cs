using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto.UserDTO
{
    public class UserReturn
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string Token { get; set; }
    }
}
