using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models
{
    public class ComboItem:BaseModel
    {
        [Key]
        public int ComboDtlId {  get; set; }
        [ForeignKey("Combos")]
        public int ComboId { get; set; }
        public Combos Combo { get; set; }

        [ForeignKey("Items")]
        public int ItemId { get; set; }
        public Items Item { get; set; }
    }
}
