namespace NanaFoodWeb.Models.Momo
{
    public class MomoRequest
    {
        public string partnerCode { get; set; } = string.Empty;
        public string? partnerName { get; set; }
        public string? storeId { get; set; }
        public string requestId { get; set; } = string.Empty;
        public string amount { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
        public string orderInfo { get; set; } = string.Empty;
        public string redirectUrl { get; set; } = string.Empty;
        public string ipnUrl { get; set; } = string.Empty;
        public string lang { get; set; }
        public string? extraData { get; set; }
        public string requestType { get; set; } = string.Empty;
        public string signature { get; set; } = string.Empty;
    }
}
