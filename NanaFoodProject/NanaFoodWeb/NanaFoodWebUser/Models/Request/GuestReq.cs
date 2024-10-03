using Helper.BaseModel;

namespace StoreManagement.Model.Request
{
    public class GuestReq : RequestData
    {
        public GuestModelReq ModelRequest { get; set; }
    }
    public class GuestModelReq
    {
        public int GuestId { get; set; }
        public string GuestLastName { get; set; }
        public string GuestFirsttName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
