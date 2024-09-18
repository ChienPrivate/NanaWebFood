using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Model
{
    [Table("CartDetails")]
    [PrimaryKey(nameof(UserId),nameof(ProductId))]
    internal class CartDetails
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public double Total { get; set;}
        public int Quantity { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
    }
}
