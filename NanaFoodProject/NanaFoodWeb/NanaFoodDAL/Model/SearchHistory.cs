using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Model
{
    [Table("SearchHistory")]
    internal class SearchHistory
    {
        [Key]
        public int SearchId { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public string SearchString { get; set; }
        public User User { get; set; }
    }
}
