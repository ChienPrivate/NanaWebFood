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
    public class OrderDetails
    {
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public bool IsReviewed { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public Review Review { get; set; }

    }
}
