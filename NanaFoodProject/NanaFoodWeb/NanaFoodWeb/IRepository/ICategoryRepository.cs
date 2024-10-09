using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface ICategoryRepository
    {
        ResponseDto GetAll(int page, int pageSize);
        ResponseDto GetById(int id);
        ResponseDto Create(Category category);
        ResponseDto Update(Category category);
        ResponseDto Delete(int id);
        ResponseDto GetByName(string name, int page, int pageSize);
        ResponseDto ModifyStatus(int id, bool status);
    }
}
