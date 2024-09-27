using NanaFoodWeb.Utility;
using static NanaFoodWeb.Utility.StaticDetails;

namespace NanaFoodWeb.Models.Dto
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
    }
}
