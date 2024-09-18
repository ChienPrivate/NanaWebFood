using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto
{
    internal class CartDetailsDto
    {
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public double Total { get; set; }
        public int Quantity { get; set; }
    }
}
