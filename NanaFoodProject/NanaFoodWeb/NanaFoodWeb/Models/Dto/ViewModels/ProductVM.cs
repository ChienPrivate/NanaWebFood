namespace NanaFoodWeb.Models.Dto.ViewModels
{
    public class ProductVM
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<Product> Products { get; set; }
    }
}
