using Newtonsoft.Json;

namespace NanaFoodWeb.Models.Dto
{
    public class CalculateShippingFeeRequestDto
    {
        [JsonProperty("service_id")]
        public int ServiceId { get; set; }

        [JsonProperty("insurance_value")]
        public int InsuranceValue { get; set; } = 50000; // phí bảo hiểm là 50k

        [JsonProperty("coupon")]
        public string Coupon { get; set; } = null;

        [JsonProperty("from_district_id")]
        public int FromDistrictId { get; set; } = 1454; // id địa chỉ của trường -> quận 12 cũng là địa chỉ quán

        [JsonProperty("to_district_id")]
        public int ToDistrictId { get; set; } // id địa chỉ giao đến

        [JsonProperty("to_ward_code")]
        public string ToWardCode { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; } = 10;

        [JsonProperty("length")]
        public int Length { get; set; } = 10;

        [JsonProperty("weight")]
        public int Weight { get; set; } = 800;

        [JsonProperty("width")]
        public int Width { get; set; } = 10;
    }
}
