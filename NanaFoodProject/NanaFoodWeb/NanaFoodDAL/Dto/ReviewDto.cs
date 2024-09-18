using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto
{
    internal class ReviewDto
    {
        public Guid ReviewId { get; set; } = Guid.NewGuid();
        public string Comment { get; set; }
        public double Rating { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
    }
}
