using Newtonsoft.Json;
namespace NanaFoodWeb.Models.Dto.GHNDto
{
    public class ExpetedShippingTimeDto
    {
        public DateTime LeadTime => DateTimeOffset.FromUnixTimeSeconds(LeadTimeUnix).DateTime;
        public DateTime OrderDate => DateTimeOffset.FromUnixTimeSeconds(OrderDateUnix).DateTime;

        [JsonProperty("leadtime")]
        public long LeadTimeUnix { get; set; }

        [JsonProperty("order_date")]
        public long OrderDateUnix { get; set; }

        // Thêm chuỗi định dạng sẵn
        public string FormattedLeadTime => LeadTime.ToLocalTime().ToString("M/d/yyyy h:mm:ss tt");
    }
}
