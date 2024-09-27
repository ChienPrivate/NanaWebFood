using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanaFoodDAL.Model
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
