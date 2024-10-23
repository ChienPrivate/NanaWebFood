namespace NanaFoodWeb.Models.Dto
{
    public class DistrictDto
    {
        public int DistrictID { get; set; }         // ID của quận/huyện
        public int ProvinceID { get; set; }         // ID của tỉnh/thành phố
        public string DistrictName { get; set; }    // Tên của quận/huyện
        public string Code { get; set; }            // Mã code của quận/huyện
        public int Type { get; set; }               // Loại (có thể là huyện, thành phố, etc.)
        public int SupportType { get; set; }        // Loại hỗ trợ
        public List<string> NameExtension { get; set; }  // Danh sách các tên mở rộng
        public bool IsEnable { get; set; }          // Trạng thái bật/tắt của quận/huyện
        public int UpdatedBy { get; set; }          // ID của người cập nhật
        public string CreatedAt { get; set; }     // Ngày tạo
        public string UpdatedAt { get; set; }     // Ngày cập nhật
        public bool CanUpdateCOD { get; set; }      // Có thể cập nhật COD hay không
        public int Status { get; set; }             // Trạng thái của quận/huyện
        public int PickType { get; set; }           // Loại lấy hàng
        public int DeliverType { get; set; }        // Loại giao hàng
        public WhiteListClientDto WhiteListClient { get; set; }  // Danh sách trắng cho client
        public WhiteListDistrictDto WhiteListDistrict { get; set; }  // Danh sách trắng cho quận/huyện
        public string ReasonCode { get; set; }      // Mã lý do
        public string ReasonMessage { get; set; }   // Thông điệp lý do
        public List<string> OnDates { get; set; } // Danh sách các ngày cập nhật
        public string UpdatedIP { get; set; }       // Địa chỉ IP của người cập nhật
        public int UpdatedEmployee { get; set; }    // Nhân viên cập nhật
        public string UpdatedSource { get; set; }   // Nguồn cập nhật
        public string UpdatedDate { get; set; }   // Ngày cập nhật
    }

    public class WhiteListClientDto
    {
        public List<string> From { get; set; }    // Danh sách client 'From'
        public List<string> To { get; set; }      // Danh sách client 'To'
        public List<string> Return { get; set; }  // Danh sách client 'Return'
    }

    public class WhiteListDistrictDto
    {
        public object From { get; set; }          // Danh sách 'From' cho quận/huyện (có thể là null)
        public object To { get; set; }            // Danh sách 'To' cho quận/huyện (có thể là null)
    }
}
