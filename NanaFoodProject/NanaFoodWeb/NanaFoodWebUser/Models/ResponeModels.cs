namespace StoreManagement.Models
{
    public class ResponeModels
    {
        public bool Status { get; set; } = true;
        public string ErrorMessage { get; set; }
        public dynamic Data { get; set; }
        public ResponeModels()
        {

        }
        public ResponeModels(string errMessage)
        {
            Status = false;
            ErrorMessage = errMessage;
        }
    }
}
