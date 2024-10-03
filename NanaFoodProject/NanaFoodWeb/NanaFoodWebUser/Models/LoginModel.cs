using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Input Login ID")]
        //[Display(XName = "Login ID")]
        public string LoginID { get; set; }
        [Required(ErrorMessage = "Input Password")]
        //[Display(Name = "Password")]
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}
