using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto.UserDTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        public string Password { get; set; }

        public bool keepLogined { get; set; } = false;
    }
}
