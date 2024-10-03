using Helper.BaseModel;

namespace StoreManagement.Model.Request
{
    public class CustomerReq : RequestData
    {
        public CustomerModelReq ModelRequest { get; set; }
    }
    public class CustomerModelReq
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerCd { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
