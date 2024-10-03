namespace StoreManagement.Models.Request
{
    public class UpdateQuantiyReq
    {
        public int DtlId { get; set; }
        public int Quantity { get; set; }
        public string CustomerCode { get; set; }
    }
}
