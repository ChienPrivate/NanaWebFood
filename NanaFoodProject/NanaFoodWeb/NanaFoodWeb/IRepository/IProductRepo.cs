using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models;

namespace NanaFoodWeb.IRepository
{
    public interface IProductRepo
    {
        ResponseDto GetAll(int page, int pageSize);
        ResponseDto GetById(int id);
        ResponseDto Create(Product product);
        ResponseDto Update(ProductDto product);
        ResponseDto Delete(int id);
        ResponseDto GetByCategoryId(int categoryid, int page, int pageSize);
        ResponseDto GetBySearch(string query, int page, int pageSize);
        ResponseDto GetByFilter(double? minrange, double? maxrange, int page, int pageSize);
        ResponseDto TopViewed(int page, int pageSize);
        ResponseDto Sorting(string sort, int page, int pageSize);
        ResponseDto ModifyStatus(int id, bool status);

    }
}
