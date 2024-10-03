using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models
{
    public class Items:BaseModel
    {
        [Key]
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }
        [NotMapped]
        public string UrlImg { get; set; }

        public virtual Categories Category { get; set; }
        public virtual ICollection<ItemImage> ItemImages { get; set; }
        public virtual ICollection<ItemMenuDetail> ItemMenuDetails { get; set; }
        public virtual ICollection<ItemDetails> ItemDetails { get; set; }
        public virtual ICollection<ComboItem> ComboItems { get; set; }
    }
}
