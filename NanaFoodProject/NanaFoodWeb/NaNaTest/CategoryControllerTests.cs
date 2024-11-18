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
        public void GetCategoryById_NonExistingId_ReturnsNotFound()
        {
            int nonExistingId = 99;
            var mockResponse = new ResponseDto { IsSuccess = false };

            _mockCategoryRepo.Setup(repo => repo.GetById(nonExistingId)).Returns(mockResponse);

            var result = _controller.GetCategoryById(nonExistingId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsBadRequest_WhenRequestFails()
        {
            var errorResponse = new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to retrieve categories."
            };

            _mockCategoryRepo.Setup(repo => repo.GetAll(1, 10, true)).ReturnsAsync(errorResponse);

            var result = await _controller.GetAllCategories(1, 10, true);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var responseDto = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.False(responseDto.IsSuccess);
            Assert.Equal("Failed to retrieve categories.", responseDto.Message);
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

        [Fact]
        public void UpdateCategory_ReturnsOkResponse_WhenUpdateIsSuccessful()
        {            
            var categoryDto = new CategoryDto
            {
                CategoryId = 1,
                CategoryName = "Updated Category",
                Description = "Updated Description",
                CategoryImage = "https://example.com/new-image.jpg",
                IsActive = true
            };
            var successfulResponse = new ResponseDto
            {
                IsSuccess = true,
                Message = "Update successful",
                Result = categoryDto
            };
            _mockCategoryRepo.Setup(repo => repo.Update(It.IsAny<Category>())).Returns(successfulResponse);           
            var result = _controller.UpdateCategory(categoryDto.CategoryId, categoryDto);
            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var responseDto = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(responseDto.IsSuccess);
            Assert.Equal("Update successful", responseDto.Message);
            Assert.NotNull(responseDto.Result);
        }

        [Fact]
        public void UpdateCategory_ReturnsBadRequest_WhenModelStateIsInvalid()
        {            
            var categoryDto = new CategoryDto
            {
                CategoryId = 1,
                CategoryName = "Invalid Category"
            };
            _controller.ModelState.AddModelError("CategoryName", "Required");
           
            var result = _controller.UpdateCategory(categoryDto.CategoryId, categoryDto);
           
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Đầu vào không hợp lệ.", badRequestResult.Value);
        }

        [Fact]
        public void UpdateCategory_ReturnsBadRequest_WhenCategoryIdMismatch()
        {           
            var categoryDto = new CategoryDto
            {
                CategoryId = 1,
                CategoryName = "Updated Category",
                Description = "Updated Description",
                CategoryImage = "https://example.com/new-image.jpg",
                IsActive = true
            };           
            var result = _controller.UpdateCategory(2, categoryDto);   
            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Đầu vào không hợp lệ.", badRequestResult.Value);
        }

        [Fact]
        public void UpdateCategory_ReturnsBadRequest_WhenUpdateFails()
        {           
            var categoryDto = new CategoryDto
            {
                CategoryId = 1,
                CategoryName = "Updated Category",
                Description = "Updated Description",
                CategoryImage = "https://example.com/new-image.jpg",
                IsActive = true
            };

            var failedResponse = new ResponseDto
            {
                IsSuccess = false,
                Message = "Update failed"
            };

            _mockCategoryRepo.Setup(repo => repo.Update(It.IsAny<Category>())).Returns(failedResponse);
            
            var result = _controller.UpdateCategory(categoryDto.CategoryId, categoryDto);
            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var responseDto = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.False(responseDto.IsSuccess);
            Assert.Equal("Update failed", responseDto.Message);
        }
    }
}
