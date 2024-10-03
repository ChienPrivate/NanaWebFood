using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models
{
    public class ItemImage:BaseModel
    {
        public int ImageId { get; set; }
        public string ImageURL { get; set; }
        public bool ImageBackground { get; set; }
        [ForeignKey("Items")]
        public int ItemId { get; set; }
        public virtual Items Item { get; set; }
    }
}
