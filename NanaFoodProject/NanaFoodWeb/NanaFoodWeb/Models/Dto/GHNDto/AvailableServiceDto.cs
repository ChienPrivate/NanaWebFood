using Newtonsoft.Json;

namespace NanaFoodWeb.Models.Dto.GHNDto
{
    public class AvailableServiceDto
    {
        [JsonProperty("service_id")]
        public int ServiceId { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("service_type_id")]
        public int ServiceTypeId { get; set; }

        [JsonProperty("config_fee_id")]
        public string ConfigFeeId { get; set; }

        [JsonProperty("extra_cost_id")]
        public string ExtraCostId { get; set; }

        [JsonProperty("standard_config_fee_id")]
        public string StandardConfigFeeId { get; set; }

        [JsonProperty("standard_extra_cost_id")]
        public string StandardExtraCostId { get; set; }

        [JsonProperty("ecom_config_fee_id")]
        public int EcomConfigFeeId { get; set; }

        [JsonProperty("ecom_extra_cost_id")]
        public int EcomExtraCostId { get; set; }

        [JsonProperty("ecom_standard_config_fee_id")]
        public int EcomStandardConfigFeeId { get; set; }

        [JsonProperty("ecom_standard_extra_cost_id")]
        public int EcomStandardExtraCostId { get; set; }
    }
}
