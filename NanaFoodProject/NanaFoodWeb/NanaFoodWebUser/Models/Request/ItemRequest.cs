using Helper.BaseModel;

namespace StoreManagement.Models.Request
{
    public class ItemRequest : RequestData
    {
        public ItemMasterReq ModelRequest { get; set; }
    }
    public class ItemMasterReq
    {
        public int ItemId { get; set; } = 0;
        public string ItemName { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public decimal Price { get; set; } = 0;
        public int CategoryId { get; set; } = 0;
        public bool Actvie { get; set; } = true;
    }
}
