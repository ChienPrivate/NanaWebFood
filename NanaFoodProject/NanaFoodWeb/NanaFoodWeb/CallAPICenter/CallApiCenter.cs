using NanaFoodWeb.IRepository;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace NanaFoodWeb.CallAPICenter
{

    public class CallApiCenter
    {
        public CallApiCenter() { }
        private string URLAPI = "https://nanafoodapi20241110164928.azurewebsites.net/api/";
        public async Task<T> GetMethod<T>(string apiName,string token)
        {
            //var res = new ResponeModel();
            using (HttpClient client = new HttpClient())
            {
                //HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                /*client.DefaultRequestHeaders.TryAddWithoutValidation("x-token", token);*/
                string domain = URLAPI + apiName;
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(60);
                    //var data = await client.PostAsync(domain, httpContent);
                    var data = await client.GetAsync(domain);

                    var jsonResponse = await data.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(jsonResponse);
                }
                catch (Exception e)
                {
                    throw new Exception($"System error: {e.Message}");
                }
            }
        }

        public async Task<T> PostMethod<T>(dynamic modelRequest, string apiName, string token)
        {
            //token = Ultils.Token;
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            //string domainIF = AppSetting.domainIF;
            using (HttpClient client = new HttpClient(handler))
            {
                string req = JsonConvert.SerializeObject(modelRequest);
                HttpContent httpContent = new StringContent(req, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //client.DefaultRequestHeaders.TryAddWithoutValidation("x-token", token);
                string domain = URLAPI + apiName;

                try
                {
                    client.Timeout = TimeSpan.FromMinutes(60);
                    var data = await client.PostAsync(domain, httpContent);

                    var jsonResponse = await data.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(jsonResponse);
                }
                catch (Exception e)
                {
                    throw new Exception($"System error: {e.Message}");
                    //return new ResponeModel("EX001","Lỗi hệ thống khi call function của IF " + (e.InnerException == null ? e.Message : e.InnerException.Message));
                }
            }
            //return new ResponseModel.ResponseModel();

        }

        public async Task<T> PutMethod<T>(dynamic modelRequest, string apiName, string token)
        {
            //string domainIF = AppSetting.domainIF;
            using (HttpClient client = new HttpClient())
            {
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(modelRequest), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string domain = URLAPI + apiName;
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(60);
                    var data = await client.PutAsync(domain, httpContent);

                    var jsonResponse = await data.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(jsonResponse);
                }
                catch (Exception e)
                {
                    throw new Exception($"System error: {e.Message}");
                    //return new ResponeModel("EX001", "Lỗi hệ thống khi call function của IF " + (e.InnerException == null ? e.Message : e.InnerException.Message));
                }
            }
            //return new ResponseModel.ResponseModel();

        }

        public async Task<T> DeleteMethod<T>(string apiName, string token)
        {
           
            //var res = new ResponeModel();
            using (HttpClient client = new HttpClient())
            {
                //HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string domain = URLAPI + apiName;
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(60);
                    //var data = await client.PostAsync(domain, httpContent);
                    var data = await client.DeleteAsync(domain);

                    var jsonResponse = await data.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(jsonResponse);
                }
                catch (Exception e)
                {
                    throw new Exception($"System error: {e.Message}");
                    //return new ResponeModel("EX001", "Lỗi hệ thống khi call function của IF " + (e.InnerException == null ? e.Message : e.InnerException.Message));
                }
            }
        }
    }
}
