namespace NanaFoodWeb.Models.Dto
{
    public class Result<T>
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public T Data { get; set; }
    }
}
