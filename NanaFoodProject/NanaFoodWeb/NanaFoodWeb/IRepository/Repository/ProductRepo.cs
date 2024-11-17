using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models;
using NanaFoodWeb.Utility;
using Humanizer;
using Newtonsoft.Json;

namespace NanaFoodWeb.IRepository.Repository
{
    public class ProductRepo(IBaseService baseService) : IProductRepo
    {
        readonly IBaseService _baseService = baseService;
        public ResponseDto Create(Product product)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.APIBase + $"/api/Product",
                Data = product
            }).Result;
        }

        public ResponseDto Delete(int id)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = StaticDetails.APIBase + $"/api/Product/delete/{id}"

            }).Result;
        }

        public ResponseDto GetAll(int page, int pageSize, bool isSelectAll)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Product?page={page}&pageSize={pageSize}&isSelectAll={isSelectAll}"

            }).Result;
        }

        public async Task<ResponseDto> GetByCategoryId(int categoryid, int page, int pageSize)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Product/FilterCategoryID/{categoryid}?page={page}&pageSize={pageSize}"
            });
        }
        public ResponseDto Update(ProductDto product)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Url = StaticDetails.APIBase + $"/api/Product/update",
                Data = product
            }).Result;
        }
        public ResponseDto GetById(int id)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Product/getbyId/{id}"
            }).Result;
        }
        public ResponseDto GetByFilter(double? minrange, double? maxrange, int page, int pageSize)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Product/Filter?minrange={minrange}&maxrange={maxrange}&page={page}&pageSize={pageSize}"
            }).Result;
        }

        

        public ResponseDto GetBySearch(string query, int page, int pageSize)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Product/Get-by-name?query={query}&page={page}&pageSize={pageSize}",
            }).Result;
        }
        public ResponseDto UnActiveCategory(int id)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Url = StaticDetails.APIBase + $"/api/Product/Unactive/{id}"
            }).Result;
        }
        public ResponseDto ModifyStatus(int id, bool status)
        {
            throw new NotImplementedException();
        }

        public ResponseDto Sorting(string sort, int page, int pageSize)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Product/Sorting?sort={sort}&page={page}&pageSize={pageSize}"
            }).Result;
        }

        public ResponseDto TopViewed(int page, int pageSize)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Product/Get-top-view?page={page}&pageSize={pageSize}"
            }).Result;
        }

        public async Task<ResponseDto> GetByCategoryIdExcludeSameProduct(int productId, int categoryid, int page, int pageSize)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Product/ExcludeSameProduct/{productId}?categoryid={categoryid}&page={page}&pageSize={pageSize}"
            });
        }

        public async Task<ResponseDto> GetProduct()
        {
            // Thực hiện gọi bất đồng bộ
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + "/api/Product/get_product"
            });
        }

        public async Task<List<string>> GetImages(int ProductId)
        {
            var response = await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,

                Url = StaticDetails.APIBase + $"/api/Product/get_product_images/{ProductId}"

            });
            var reuslt = JsonConvert.DeserializeObject<List<string>>(response.Result.ToString());

            return reuslt;
        }
    }
}
