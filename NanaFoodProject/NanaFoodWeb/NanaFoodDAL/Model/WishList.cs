using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanaFoodDAL.Model
{
    [Table("WishList")]
    [PrimaryKey(nameof(UserId),nameof(ProductId))]
    public class WishList
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
    }
}
