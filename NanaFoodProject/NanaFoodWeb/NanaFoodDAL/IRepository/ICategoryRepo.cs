using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;

namespace NanaFoodDAL.IRepository
{
    public interface ICategoryRepo
    {
        ResponseDto GetAll(int page, int pageSize);
        ResponseDto GetById(int id);
        ResponseDto Create(Category category);
        ResponseDto Update(Category category);
        ResponseDto Delete(int id);
        ResponseDto GetByName(string name, int page, int pageSize);
        ResponseDto ModifyStatus(int id,bool status);
    }
}
