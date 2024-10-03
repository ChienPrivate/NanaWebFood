using Helper.BaseModel;

namespace StoreManagement.Models.Request
{
    public class RegisterReq : RequestData
    {
        public RegisterModelReq ModelRequest { get; set; }
    }
    public class RegisterModelReq
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        //public string UserLevel { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public bool Sex { get; set; }
    }
}
