using Moq;
using Xunit;
using NanaFoodApi.Controllers;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Model;
using Microsoft.AspNetCore.Routing;

namespace NaNaTest
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryRepo> _mockCategoryRepo;
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockCategoryRepo = new Mock<ICategoryRepo>();
            _mockProductRepo = new Mock<IProductRepository>();
            _controller = new CategoryController(_mockCategoryRepo.Object, _mockProductRepo.Object);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsOkResult_WithCategoryList()
        {           
            var mockResponse = new ResponseDto
            {
                IsSuccess = true,
                Result = new List<CategoryDto>
                {
                    new CategoryDto { CategoryId = 1, CategoryName = "Beverages", IsActive = true },
                    new CategoryDto { CategoryId = 2, CategoryName = "Snacks", IsActive = true }
                }
            };
            _mockCategoryRepo.Setup(repo => repo.GetAll(1, 10, true)).ReturnsAsync(mockResponse);
           
            var result = await _controller.GetAllCategories();
            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.NotEmpty((List<CategoryDto>)returnValue.Result);
        }

        [Fact]
        public void GetCategoryById_CategoryExists_ReturnsOkResult()
        {           
            var mockResponse = new ResponseDto
            {
                IsSuccess = true,
                Result = new CategoryDto { CategoryId = 1, CategoryName = "Beverages", IsActive = true }
            };
            _mockCategoryRepo.Setup(repo => repo.GetById(1)).Returns(mockResponse);
            
            var result = _controller.GetCategoryById(1);
            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal("Beverages", ((CategoryDto)returnValue.Result).CategoryName);
        }

        [Fact]
        public void CreateCategory_ValidCategory_ReturnsCreatedAtAction()
        {           
            var categoryDto = new CategoryDto
            {
                CategoryId = 3,
                CategoryName = "New Category",
                Description = "Test description",
                IsActive = true
            };

            var category = new Category
            {
                CategoryId = categoryDto.CategoryId,
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                IsActive = categoryDto.IsActive
            };

            var mockResponse = new ResponseDto
            {
                IsSuccess = true,
                Result = categoryDto
            };

            _mockCategoryRepo.Setup(repo => repo.Create(It.IsAny<Category>())).Returns(mockResponse);
            
            var result = _controller.CreateCategory(categoryDto);
            
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<ResponseDto>(createdResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal("New Category", ((CategoryDto)returnValue.Result).CategoryName);
        }

        [Fact]
        public void DeleteCategory_ValidCategoryId_ReturnsOkResult()
        {           
            var mockResponse = new ResponseDto { IsSuccess = true, Message = "Category deleted successfully" };
            _mockCategoryRepo.Setup(repo => repo.Delete(1)).Returns(mockResponse);
            
            var result = _controller.DeleteCategory(1);
            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal("Category deleted successfully", returnValue.Message);
        }

        [Fact]
        public void GetCategoryById_ExistingId_ReturnsOk()
        {          
            int categoryId = 1;
            var mockResponse = new ResponseDto
            {
                IsSuccess = true,
                Result = new CategoryDto
                {
                    CategoryId = categoryId,
                    CategoryName = "Category",
                    Description = "Test description",
                    IsActive = true
                }
            };

            _mockCategoryRepo.Setup(repo => repo.GetById(categoryId)).Returns(mockResponse);
           
            var result = _controller.GetCategoryById(categoryId);
            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal("Category", ((CategoryDto)returnValue.Result).CategoryName);
        }

        [Fact]
        public async Task GetCategoryByName_ValidName_ReturnsOk()
        {
            
            var mockResponse = new ResponseDto
            {
                IsSuccess = true,
                Result = new List<CategoryDto>
                {
                    new CategoryDto { CategoryId = 1, CategoryName = "Food", IsActive = true },
                    new CategoryDto { CategoryId = 2, CategoryName = "Drink", IsActive = true }
                }
            };

            _mockCategoryRepo.Setup(repo => repo.GetByName("Food", 1, 10)).ReturnsAsync(mockResponse);
           
            var result = await _controller.GetCategoryByName("Food", 1, 10);
           
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.IsType<List<CategoryDto>>(returnValue.Result);
        }

        [Fact]
        public void DeleteCategory_ValidId_ReturnsOk()
        {
            
            int categoryId = 1;
            var mockResponse = new ResponseDto
            {
                IsSuccess = true,
                Message = "Xóa danh mục thành công"
            };

            _mockCategoryRepo.Setup(repo => repo.Delete(categoryId)).Returns(mockResponse);
           
            var result = _controller.DeleteCategory(categoryId);
           
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal("Xóa danh mục thành công", returnValue.Message);
        }

        [Fact]
        public void GetCategoryById_NonExistingId_ReturnsNotFound()
        {
            int nonExistingId = 99;
            var mockResponse = new ResponseDto { IsSuccess = false };

            _mockCategoryRepo.Setup(repo => repo.GetById(nonExistingId)).Returns(mockResponse);

            var result = _controller.GetCategoryById(nonExistingId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllCategories_NoCategories_ReturnsOkWithEmptyList()
        {
            var mockResponse = new ResponseDto
            {
                IsSuccess = true,
                Result = new List<CategoryDto>() 
            };

            _mockCategoryRepo.Setup(repo => repo.GetAll(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(mockResponse);

            var result = await _controller.GetAllCategories();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Empty((IEnumerable<CategoryDto>)returnValue.Result); 
        }

        [Fact]
        public void UpdateCategory_ExistingId_ReturnsOk()
        {
            var existingCategoryId = 1;
            var categoryDto = new CategoryDto
            {
                CategoryId = existingCategoryId,
                CategoryName = "Updated Category",
                Description = "Updated Description",
                CategoryImage = "https://updated-image.jpg",
                IsActive = true
            };

            var mockResponse = new ResponseDto { IsSuccess = true, Result = categoryDto };

            _mockCategoryRepo.Setup(repo => repo.Update(It.IsAny<Category>())).Returns(mockResponse);

            var result = _controller.UpdateCategory(existingCategoryId, categoryDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(returnValue.IsSuccess);

            var updatedCategoryDto = Assert.IsType<CategoryDto>(returnValue.Result);
            Assert.Equal(categoryDto.CategoryName, updatedCategoryDto.CategoryName);
        }

        [Fact]
        public void GetFoods_WhenNoData_ReturnsEmptyList()
        {
            _mockProductRepo.Setup(x => x.Products).Returns(new List<Product>());

            var result = _controller.GetFoods();

            var okResult = result as OkObjectResult;
            var response = okResult.Value as ResponseDto;
            Assert.True(response.IsSuccess);
            Assert.Equal(0, (response.Result as IEnumerable<object>).Count());
        }
    }
}
