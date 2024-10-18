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
    internal class CartDetailsDto
    {
        [Required(ErrorMessage = "Không được để trống UserId")]
        public string UserId { get; set; }
        public int ProductId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double Total { get; set; }
        [Range(1,int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}
