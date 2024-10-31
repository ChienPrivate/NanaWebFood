using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;

namespace NanaFoodDAL.IRepository.Repository
{
    internal class ProductRepository(ApplicationDbContext context, IMapper mapper) : IProductRepository
    {
        ApplicationDbContext _context = context;
        IMapper _mapper = mapper;
        ResponseDto response = new ResponseDto();

        public IEnumerable<Product> Products => _context.Products.ToList();

        public ResponseDto Create(Product product)
        {
            try
            {
                var checkCategoryExist = _context.Categories.Find(product.CategoryId);
                if (checkCategoryExist == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã loại món không tồn tại";
                    return response;
                }
                if (product == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Đầu vào không hợp lệ";
                    return response;
                }
                if (_context.Products.Any(p => p.ProductName == product.ProductName))
                {
                    response.IsSuccess = false;
                    response.Message = $"Món ăn '{product.ProductName}' đã tồn tại";
                    return response;
                }
                _context.Products.Add(product);
                _context.SaveChanges();
                response.Result = _mapper.Map<ProductDto>(product);
                response.Message = "Thêm món ăn thành công";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ResponseDto Delete(int id)
        {
            try
            {
                var product = _context.Products.Find(id);

                // Check exist
                if (product == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Món ăn này không tồn tại";
                    return response;
                }

                _context.Products.Remove(product);
                _context.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Xoá món ăn thành công";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }



        public ResponseDto GetAll(int page = 1, int pageSize = 10, bool isSelectAll = true)
        {
            try
            {
                if (isSelectAll == false)
                {
                    var ActiveProducts = _context.Products.Where(p => p.IsActive == true).ToList();
                    var totalCountActiveProducts = ActiveProducts.Count;
                    var totalPagesActiveProducts = (int)Math.Ceiling((decimal)totalCountActiveProducts / pageSize);

                    var productsPerPage2 = ActiveProducts
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    response.Result = new
                    {
                        TotalCount = totalCountActiveProducts,
                        TotalPages = totalPagesActiveProducts,
                        Products = _mapper.Map<List<ProductDto>>(productsPerPage2)
                    };
                    response.IsSuccess = true;
                    response.Message = "Lấy danh sách món ăn thành công.";

                    return response;
                }
                var products = _context.Products.ToList();
                var totalCount = products.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                var productsPerPage = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Products = _mapper.Map<List<ProductDto>>(productsPerPage)
                };
                response.IsSuccess = true;
                response.Message = "Lấy danh sách món ăn thành công.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ResponseDto GetByCategoryId(int categoryid, int page, int pageSize)
        {
            try
            {
                var products = _context.Products.Where(p => p.CategoryId == categoryid).ToList();
                var totalCount = products.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var productsPerPage = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Products = _mapper.Map<List<ProductDto>>(productsPerPage)
                };
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ResponseDto GetByFilter(double? minrange, double? maxrange, int page, int pageSize)
        {
            try
            {
                var products = _context.Products.ToList().AsQueryable();
                if (minrange.HasValue && maxrange.HasValue)
                {
                    products = products.Where(x => x.Price <= maxrange.Value && x.Price >= minrange.Value);
                }
                var productFiltered = products.ToList();
                var totalCount = productFiltered.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var productsPerPage = productFiltered
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Products = _mapper.Map<List<ProductDto>>(productsPerPage)
                };
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ResponseDto GetById(int id)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Món ăn này không tồn tại";
                    return response;
                }
                product.View += 1;
                _context.SaveChanges();
                response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ResponseDto GetBySearch(string query, int page, int pageSize)
        {
            try
            {
                var products = _context.Products.Where(p => p.ProductName.Contains(query)).ToList();
                var totalCount = products.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var productsPerPage = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Products = _mapper.Map<List<ProductDto>>(productsPerPage)
                };
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ResponseDto ModifyStatus(int id, bool status)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Mã món ăn không tồn tại";
                    return response;
                }
                product.IsActive = status;
                _context.SaveChangesAsync();
                response.Message = "Cập nhật trạng thái thành công";
                response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }
            return response;
        }

        public ResponseDto Sorting(string sort, int page = 1, int pageSize = 10)
        {
            try
            {
                List<Product> products;
                if (sort == "desc")
                {
                    products = _context.Products.Where(p => p.IsActive).OrderByDescending(x => x.Price).ToList();
                }
                else
                {
                    products = _context.Products.Where(p => p.IsActive).OrderBy(x => x.Price).ToList();
                }
                var totalCount = products.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var productsPerPage = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Products = _mapper.Map<List<ProductDto>>(productsPerPage)
                };
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ResponseDto TopViewed(int page = 1, int pageSize = 10)
        {
            try
            {
                var products = _context.Products.OrderByDescending(x => x.View).ToList();
                var totalCount = products.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var productsPerPage = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                response.Result = _mapper.Map<List<ProductDto>>(productsPerPage);
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ResponseDto Update(Product product)
        {
            try
            {
                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();
                response.Result = _mapper.Map<ProductDto>(product);
                response.Message = "Cập nhật món ăn thành công";
            }
            catch
            {
                response.IsSuccess = false;
                response.Message = "Loại món này không tồn tại";
            }
            return response;
        }

        public async Task<ResponseDto> GetByCategoryIdExcludeSameProduct(int productId,int categoryid, int page, int pageSize)
        {
            try
            {
                var products = await _context.Products.Where(p => p.CategoryId == categoryid && p.ProductId != productId).ToListAsync();
                var totalCount = products.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var productsPerPage = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Products = _mapper.Map<List<ProductDto>>(productsPerPage)
                };
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public  ResponseDto GetProduct()
        {
            var product = _context.Products.ToList(); 
            if(product == null)
            {
                response.IsSuccess= false;
                response.Message = "Không có sản phẩm để hiển  thị.";
                return response;
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "Đã hiển thị tất cả sản phẩm.";
                response.Result = product;
            }
           
            return response;
        }
    }
}
