using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "không được để trống tên")]
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public string? CategoryImage { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
