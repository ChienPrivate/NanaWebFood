namespace StoreManagement.Models
{
    public class ItemMenu : BaseModel
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public int CategoryId { get; set; }
        public decimal Price {  get; set; }
        public virtual ICollection<ItemMenuDetail> ItemMenuDetails { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
