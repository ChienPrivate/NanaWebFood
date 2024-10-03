using Helper.BaseModel;

namespace StoreManagement.Model.Request
{
    public class CategoriesReq : RequestData
    {
        public CategoriesModelReq ModelRequest { get; set; }
    }
    public class CategoriesModelReq
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }=String.Empty;
    }
}
