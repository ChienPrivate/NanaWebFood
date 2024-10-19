using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanaFoodDAL.Model
{
    [Table("CartDetails")]
    [PrimaryKey(nameof(UserId),nameof(ProductId))]
    public class CartDetails
    {
        [ForeignKey(nameof(UserId))]
        public string UserId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public int ProductId { get; set; }
        public double Total { get; set;}
        public int Quantity { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
    }
}
