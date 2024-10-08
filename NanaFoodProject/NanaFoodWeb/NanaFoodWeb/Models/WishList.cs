using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanaFoodWeb.Models
{
    public class WishList
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
    }
}
