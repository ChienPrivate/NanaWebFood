using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models
{
    public class ItemDetails:BaseModel
    {
        public int ItemDtId { get; set; }
        [ForeignKey("Items")]
        public int ItemId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual Items Item { get; set; }
    }
}
