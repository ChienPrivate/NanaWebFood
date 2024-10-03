using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models
{
    public class Combos:BaseModel
    {
        [Key]
        public int ComboId { get; set; }
        public string ComboName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public virtual ICollection<ComboItem> ComboItems { get; set; } //những sp trong combo
    }
}
