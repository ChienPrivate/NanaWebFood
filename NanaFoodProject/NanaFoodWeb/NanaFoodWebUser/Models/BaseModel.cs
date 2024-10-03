namespace StoreManagement.Models
{
    public class BaseModel
    {
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }
    }
}
