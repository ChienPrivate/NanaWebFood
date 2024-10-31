namespace NanaFoodWeb.Models.Dto
{
    public class UserReviewDto
    {
        public string ReviewId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string? Comment { get; set; }
        public double? Rating { get; set; }
        public DateTime ReviewedDate { get; set; }
    }
}
