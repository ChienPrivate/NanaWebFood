namespace NanaFoodWeb.Models.Dto.ViewModels
{
    public class ReviewVM
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<UserReviewDto> Reviews { get; set; }
    }
}
