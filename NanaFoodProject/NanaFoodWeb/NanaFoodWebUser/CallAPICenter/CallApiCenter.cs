using Helper.BaseModel;
using Newtonsoft.Json;
using StoreManagement.Models;
using StoreManagement.Utils;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace StoreManagement.CallAPICenter
{

    public class CallApiCenter
    {
        public CallApiCenter() { }

        public async Task<ResponeModel> GetMethod(string apiName,string token)
        {
            var res = new ResponeModel();
            using (HttpClient client = new HttpClient())
            {
                //HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("x-token", token);
                string domain = Ultils.URLAPI + apiName;
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(60);
                    //var data = await client.PostAsync(domain, httpContent);
                    var data = await client.GetAsync(domain);

                    var jsonResponse = await data.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ResponeModel>(jsonResponse);
                }
                catch (Exception e)
                {
                    return new ResponeModel("EX001","Lỗi hệ thống khi call function của IF " + (e.InnerException == null ? e.Message : e.InnerException.Message));
                }
            }
        }

        public async Task<ResponeModel> PostMethod(dynamic modelRequest, string apiName, string token)
        {
            token = Ultils.Token;
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            //string domainIF = AppSetting.domainIF;
            using (HttpClient client = new HttpClient(handler))
            {
                string req = JsonConvert.SerializeObject(modelRequest);
                HttpContent httpContent = new StringContent(req, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                //client.DefaultRequestHeaders.TryAddWithoutValidation("x-token", token);
                string domain = Ultils.URLAPI + apiName;

                try
                {
                    client.Timeout = TimeSpan.FromMinutes(60);
                    var data = await client.PostAsync(domain, httpContent);

                    var jsonResponse = await data.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ResponeModel>(jsonResponse);
                }
                catch (Exception e)
                {
                    return new ResponeModel("EX001","Lỗi hệ thống khi call function của IF " + (e.InnerException == null ? e.Message : e.InnerException.Message));
                }
            }
            //return new ResponseModel.ResponseModel();

        }

        public static async Task<ResponeModel> PutMethod(RequestData modelRequest, string apiName, string token)
        {
            //string domainIF = AppSetting.domainIF;
            using (HttpClient client = new HttpClient())
            {
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(modelRequest), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("x-token", token);
                string domain = Ultils.URLAPI + apiName;
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(60);
                    var data = await client.PutAsync(domain, httpContent);

                    var jsonResponse = await data.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ResponeModel>(jsonResponse);
                }
                catch (Exception e)
                {
                    return new ResponeModel("EX001", "Lỗi hệ thống khi call function của IF " + (e.InnerException == null ? e.Message : e.InnerException.Message));
                }
            }
            //return new ResponseModel.ResponseModel();

        }


    }
}
