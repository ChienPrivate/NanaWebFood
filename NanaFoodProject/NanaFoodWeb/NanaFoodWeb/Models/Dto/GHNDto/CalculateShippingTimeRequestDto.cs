using Newtonsoft.Json;

namespace NanaFoodWeb.Models.Dto.GHNDto
{
    public class CalculateShippingTimeRequestDto
    {
        [JsonProperty("from_district_id")]
        public int FromDistrictId { get; set; }

        [JsonProperty("from_ward_code")]
        public string FromWardCode { get; set; }

        [JsonProperty("to_district_id")]
        public int ToDistrictId { get; set; }

        [JsonProperty("to_ward_code")]
        public string ToWardCode { get; set; }

        [JsonProperty("service_id")]
        public int ServiceId { get; set; }
    }
}
