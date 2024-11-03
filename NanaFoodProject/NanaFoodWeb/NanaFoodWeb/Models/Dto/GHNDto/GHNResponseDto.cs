namespace NanaFoodWeb.Models.Dto.GHNDto
{
    public class GHNResponseDto<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
