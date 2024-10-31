using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto
{
    class ReviewProductDto
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public string? ProductImage { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public string? Comment { get; set; }
        public double? Rating { get; set; }
        public string UserImageUrl { get; set; }
        public string UserFullName { get; set; }
        public bool IsReviewed { get; set; }
    }
}
