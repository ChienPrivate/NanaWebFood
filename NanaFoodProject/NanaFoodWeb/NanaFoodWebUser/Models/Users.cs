namespace StoreManagement.Models
{
    public class Users : BaseModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Role { get; set; }
        //public string UserLevel { get; set; }
    }
}
