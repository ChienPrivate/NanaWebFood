namespace NanaFoodWeb.Utility
{
    public class StaticDetails
    {
        public static string APIBase { get; set; }
        public static string GHNApiKey { get; set; }
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JWTToken";
        public const string ProvinceEndPoint = "https://online-gateway.ghn.vn/shiip/public-api/master-data/province";
        public const string DistrictEndPoint = "https://online-gateway.ghn.vn/shiip/public-api/master-data/district";
        public const string WardEndPoint = "https://online-gateway.ghn.vn/shiip/public-api/master-data/ward";
        public const string AvailableServiceEndPoint = "https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/available-services";
        public const string ShipppingFeeCaculateEndPoint = "https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
