using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto
{
    public class RebuyOrderDto
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public double CurrentPrice { get; set; }
        public double OldPrice { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public bool IsActive { get; set; }
    }
}
