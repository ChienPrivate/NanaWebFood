namespace NanaFoodWeb.Models.Dto
{
    public class WardDto
    {
        public string WardCode { get; set; }         // Mã phường/xã
        public int DistrictID { get; set; }          // ID quận/huyện
        public string WardName { get; set; }         // Tên phường/xã
        public List<string> NameExtension { get; set; } // Các tên mở rộng
        public bool CanUpdateCOD { get; set; }       // Có thể cập nhật COD hay không
        public int SupportType { get; set; }         // Loại hỗ trợ
        public int PickType { get; set; }            // Loại lấy hàng
        public int DeliverType { get; set; }         // Loại giao hàng
        public WhiteListClientDto WhiteListClient { get; set; } // Danh sách trắng của khách hàng
        public WhiteListWardDto WhiteListWard { get; set; }     // Danh sách trắng của phường/xã
        public int Status { get; set; }              // Trạng thái
        public string ReasonCode { get; set; }       // Mã lý do
        public string ReasonMessage { get; set; }    // Thông báo lý do
        public List<string> OnDates { get; set; }    // Danh sách ngày (đã chuyển sang string)
        public string CreatedIP { get; set; }        // Địa chỉ IP tạo
        public int CreatedEmployee { get; set; }     // ID nhân viên tạo
        public string CreatedSource { get; set; }    // Nguồn tạo
        public string CreatedDate { get; set; }      // Ngày tạo (string)
        public int UpdatedEmployee { get; set; }     // ID nhân viên cập nhật
        public string UpdatedDate { get; set; }      // Ngày cập nhật (string)
    }

    public class WhiteListWardDto
    {
        public List<string> From { get; set; } = new List<string>();  // Danh sách khách hàng từ
        public List<string> To { get; set; } = new List<string>();    // Danh sách khách hàng đến
        public List<string> Return { get; set; } = new List<string>(); // Danh sách khách hàng trả
    }
}
