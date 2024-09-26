using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models
{
    [Table("SearchHistory")]
    public class SearchHistory
    {
        [Key]
        public int SearchId { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public string SearchString { get; set; }
        public User User { get; set; }
    }
}
