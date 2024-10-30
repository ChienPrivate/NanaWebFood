using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models
{
    public class Review
    {
        public Guid ReviewId { get; set; } = Guid.NewGuid();
        public string Comment { get; set; }
        public double Rating { get; set; } = 0;
        public string? UserId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public DateTime ReviewedDate { get; set; } = DateTime.Now;
    }
}
