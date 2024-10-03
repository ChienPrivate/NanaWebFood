namespace StoreManagement.Models.Request
{
    public class PaymentReq
    {
        public CustomerPayReq ModelRequest { get; set; }

    }
    public class CustomerPayReq
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCd { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        //public string PassWord { get; set; }
        public bool Sex { get; set; } = true;//0 nam, 1 nu
        public bool Guest_YN { get; set; } = false;
        public bool Ck_YN { get; set; } = false;
    }
}
