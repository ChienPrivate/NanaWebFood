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
    [Table("OrderDetails")]
    [PrimaryKey(nameof(OrderId),nameof(ProductId))]
    internal class OrderDetails
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
