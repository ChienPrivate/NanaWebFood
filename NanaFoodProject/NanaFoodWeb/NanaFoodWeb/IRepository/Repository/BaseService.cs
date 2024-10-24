using NanaFoodWeb.Models.Dto;
using static NanaFoodWeb.Utility.StaticDetails;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace NanaFoodWeb.IRepository.Repository
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory,
                           ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto> SendAsync(RequestDto requestDTO, bool withBearer = true, string schema = "")
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("NanaAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                //Token
                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                if (requestDTO.AccessToken != null)
                {
                    message.Headers.Add(schema,requestDTO.AccessToken);
                }

                message.RequestUri = new Uri(requestDTO.Url);
                // Tạo MultipartFormDataContent nếu có file
                if (requestDTO.Data is IFormFile file)
                {
                    var formContent = new MultipartFormDataContent();
                    var stream = file.OpenReadStream(); // Mở luồng để đọc file
                    var content = new StreamContent(stream);
                    content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); // Đặt ContentType của file

                    // Thêm file vào nội dung
                    formContent.Add(content, "file", file.FileName); // "file" là tên tham số trong controller

                    message.Content = formContent; // Gán nội dung cho message
                }
                else if (requestDTO.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? apiResponse = null;

                switch (requestDTO.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "404: Not Found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "403: Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "401: Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "500: Internal Sever Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();

                        ResponseDto apiResponseDTO;

                        apiResponseDTO = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

                        //if (!string.IsNullOrEmpty(apiContent) && apiResponseDTO.Result == null)
                        //{
                        //    apiResponseDTO = new ResponseDto
                        //    {
                        //        IsSuccess = true,
                        //        Message = apiResponseDTO.Message,
                        //        Result = JsonConvert.DeserializeObject<object>(apiContent) // Deserialize thành object
                        //    };
                        //}

                        return apiResponseDTO;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto()
                {
                    Message = ex.Message,
                    IsSuccess = false,
                };
                return dto;
            }
        }
    }
}
