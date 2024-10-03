using Helper.BaseModel;

namespace StoreManagement.Model.Request
{
    public class ComboReq : RequestData
    {
        public ComboModelReq ModelRequest { get; set; }
    }
    public class ComboModelReq
    {
        public int ComboId { get; set; }
        public string ComboName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
