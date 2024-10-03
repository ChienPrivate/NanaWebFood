using Helper.BaseModel;

namespace StoreManagement.Models.Request
{
    public class ItemImageReq : RequestData
    {
        public ItemImageModelReq ModelRequest { get; set; }
    }
    public class ItemImageModelReq
    {
        public int ImageId { get; set; }
        public string ImageURL { get; set; }= string.Empty;
        public bool ImageBackground { get; set; }
        public int ItemId { get; set; }
    }
}
