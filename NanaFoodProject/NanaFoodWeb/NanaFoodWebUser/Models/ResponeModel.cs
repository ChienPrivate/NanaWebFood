namespace StoreManagement.Models
{
    public class ResponeModel
    {
        public bool Status { get; set; } = true;
        public string MessageCode {  get; set; }
        public string Message {  get; set; }
        public dynamic Data {  get; set; }
        public ResponeModel() { }

        public ResponeModel(string messCode, string messsage)
        {
            this.Status = false;
            this.MessageCode = messCode;
            this.Message = messsage;
        }
    }
}
