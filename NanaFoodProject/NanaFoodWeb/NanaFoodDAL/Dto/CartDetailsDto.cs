using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto
{
    public class CartDetailsDto
    {
        public string? UserId { get; set; }
        public int ProductId { get; set; }
        [Range(1,int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
        public double? Total { get; set; }
    }
}
