using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models
{
    public class ItemMenuDetail : BaseModel
    {
        public int Id { get; set; }
        [ForeignKey("Items")]
        public int ItemMenuId {  get; set; }
        [ForeignKey("ItemMenu")]
        public int ItemId {  get; set; }
        public virtual ItemMenu ItemMenu { get; set; }
        public virtual Items Item { get; set; }
    }
}
