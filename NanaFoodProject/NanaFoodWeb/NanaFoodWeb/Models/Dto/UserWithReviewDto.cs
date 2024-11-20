namespace NanaFoodWeb.Models.Dto
{
    public class UserWithReviewDto
    {
        public string ReviewId { get; set; }
        public string UserId { get; set; }
        public string UserAvartar { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewedDate { get; set; }
        public bool IsConfirm { get; set; }
    }
}
