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
    public class ProductDto
    {
        public int ProductId { get; set; }
        [StringLength(50)]
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
        public int View { get; set; }
        [StringLength(450)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
    }
}
