using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;

namespace NanaFoodDAL.IRepository.Repository
{
    internal class CategoryRepo(ApplicationDbContext context, IMapper mapper) : ICategoryRepo
    {
        ApplicationDbContext _context = context;
        IMapper _mapper = mapper;
        ResponseDto response = new ResponseDto();
        public ResponseDto Create(Category category)
        {
            try
            {
                var checkNameExist = _context.Categories.FirstOrDefault(c => c.CategoryName == category.CategoryName);
                if (checkNameExist != null)
                {
                    response.Message = "Tên loại món này đã tồn tại";
                    response.IsSuccess = false;
                    return response;
                }
                _context.Categories.Add(category);
                _context.SaveChanges();
                //response.Result = _mapper.Map<CategoryDto>(category);
                response.Result = new
                {
                    TotalCount = 1,
                    TotalPages = 1,
                    Data = _mapper.Map<CategoryDto>(category)
                };
                response.Message = "Tạo loại món thành công.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }

            return response;
        }

        public ResponseDto Delete(int id)
        {
            try
            {
                var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
                if (category == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Loại món này không tồn tại.";
                    return response;
                }

                _context.Categories.Remove(category);
                _context.SaveChanges();
                response.Message = "Đã xoá loại món ăn.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDto> GetAll(int page = 1, int pageSize = 10, bool isSelectAll = true)
        {
            if (isSelectAll)
            {
                response.IsSuccess = true;
                response.Result = _mapper.Map<List<CategoryDto>>(await _context.Categories.ToListAsync());
                response.Message = "Lấy danh sách danh mục sản phẩm thành công";

                return response;
            }
            try
            {
                var categories = _context.Categories.ToList();
                var totalCate = categories.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCate / pageSize);

                var CatesPerPage = categories
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                response.Result = new
                {
                    TotalCount = totalCate,
                    TotalPages = totalPages,
                    Data = _mapper.Map<List<CategoryDto>>(CatesPerPage)
                };

                response.IsSuccess = true;
                response.Message = "Lấy danh sách loại món ăn thành công.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }

            return response;
        }

        public ResponseDto GetById(int id)
        {
            try
            {
                var category = _context.Categories.Find(id);
                if (category == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Loại món này không tồn tại.";
                }
                else
                {
                    response.Result = new
                    {
                        TotalCount = 1,
                        TotalPages = 1,
                        Data = _mapper.Map<CategoryDto>(category)
                    };
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDto> GetByName(string name, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return await GetAll(page, pageSize);
                }
                var categories = _context.Categories.Where(x => x.CategoryName.Contains(name) && x.IsActive).ToList();
                if (categories == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Loại món này không tồn tại.";
                }
                else
                {
                    var totalCate = categories.Count;
                    var totalPages = (int)Math.Ceiling((decimal)totalCate / pageSize);

                    var CatesPerPage = categories
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    response.Result = new
                    {
                        TotalCount = totalCate,
                        TotalPages = totalPages,
                        Data = _mapper.Map<List<CategoryDto>>(CatesPerPage)
                    };

                    response.IsSuccess = true;
                    response.Message = "Lấy danh sách loại món ăn theo tên thành công.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }

            return response;
        }

        public ResponseDto ModifyStatus(int id, bool status)
        {
            try
            {
                var category = _context.Categories.Find(id);
                if(category == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Mã loại món không tồn tại";
                    return response;
                }
                category.IsActive = status;
                _context.SaveChangesAsync();
                response.Message = "Cập nhật trạng thái thành công";
                response.Result = _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }
            return response;
        }

        public ResponseDto Update(Category category)
        {
            try
            {
                _context.Entry(category).State = EntityState.Modified;
                _context.SaveChanges();
                response.Result = _mapper.Map<CategoryDto>(category);
                response.Message = "Cập nhật loại món thành công";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }
            return response;
        }
    }
}
