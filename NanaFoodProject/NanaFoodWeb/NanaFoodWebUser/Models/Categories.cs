using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models
{
    public class Categories:BaseModel
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<Items> Items { get; set; }
    }
}
