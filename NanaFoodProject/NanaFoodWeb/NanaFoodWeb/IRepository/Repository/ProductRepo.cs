using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models;
using NanaFoodWeb.Utility;

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

        public ResponseDto GetAll(int page, int pageSize)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Product?page={page}&pageSize={pageSize}"

            }).Result;
        }

        public ResponseDto GetByCategoryId(int categoryid, int page, int pageSize)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Product/FilterCategoryID/{categoryid}?page={page}&pageSize={pageSize}"
            }).Result;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public ResponseDto TopViewed(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

       
    }
}
