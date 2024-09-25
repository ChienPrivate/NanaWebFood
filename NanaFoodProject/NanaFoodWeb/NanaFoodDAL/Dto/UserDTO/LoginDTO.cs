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
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm 1 chữ in hoa, 1 chữ thường và 1 số.")]
        public string Password { get; set; }

        public bool keepLogined { get; set; }
    }
}
