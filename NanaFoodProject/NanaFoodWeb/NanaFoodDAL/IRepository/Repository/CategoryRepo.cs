using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                _context.Categories.Add(category);
                _context.SaveChanges();
                response.Result = _mapper.Map<CategoryDto>(category);
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

        public ResponseDto GetAll()
        {
            try
            {
                var categories = _context.Categories.ToList();
                var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
                response.Result = categoryDtos;
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
                var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
                if (category == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Loại món này không tồn tại.";
                }
                else
                {
                    response.Result = _mapper.Map<CategoryDto>(category);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }

            return response;
        }

        public ResponseDto GetByName(string name)
        {
            try
            {
                var category = _context.Categories.Where(x => x.CategoryName.Contains(name)).ToList();
                if (category == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Loại món này không tồn tại.";
                }
                else
                {
                    response.Result = _mapper.Map<List<CategoryDto>>(category);
                }
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
            catch
            {
                response.IsSuccess = false;
                response.Message = "Loại món này không tồn tại";
            }
            return response;
        }
    }
}
