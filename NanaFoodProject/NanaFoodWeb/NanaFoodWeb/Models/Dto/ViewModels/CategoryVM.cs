namespace NanaFoodWeb.Models.Dto.ViewModels
{
    public class CategoryVM
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<Category> Categories { get; set; }
    }
}
