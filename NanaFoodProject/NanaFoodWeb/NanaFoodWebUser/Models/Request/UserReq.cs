using Helper.BaseModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreAPI.Model.Request
{
    public class UserReq:RequestData
    {
        public UserModelReq ModelRequest { get; set; }
    }
    public class UserModelReq
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PassWord { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
