using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using NanaFoodWeb.Controllers;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Dto.ViewModels;
using Newtonsoft.Json;

namespace NaNaTest
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepo> _mockProductRepo;
        private readonly Mock<IHelperRepository> _mockHelperRepository;
        private readonly Mock<ITokenProvider> _mockTokenProvider;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductRepo = new Mock<IProductRepo>();
            _mockHelperRepository = new Mock<IHelperRepository>();
            _mockTokenProvider = new Mock<ITokenProvider>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();

            _controller = new ProductsController(
                _mockProductRepo.Object,
                _mockHelperRepository.Object,
                _mockTokenProvider.Object,
                _mockCategoryRepository.Object
            );
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task Index_WithValidInput_ReturnsViewWithProducts()
        {
            var mockProducts = new List<Product> { new Product { ProductId = 1, ProductName = "Test Product" } };
            var mockProductVM = new ProductVM { Products = mockProducts };
            var mockResponse = new ResponseDto { IsSuccess = true, Result = JsonConvert.SerializeObject(mockProductVM) };

            _mockProductRepo.Setup(repo => repo.GetAll(1, 10, true)).Returns(mockResponse);

            var result = await _controller.Index(null, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData["lazyLoadData"] as List<Product>;

            Assert.NotNull(model);
            Assert.Equal(mockProducts.Count, model.Count);
            Assert.Equal(mockProducts[0].ProductName, model.First().ProductName);
        }

        [Fact]
        public async Task Index_WithInvalidInput_ReturnsEmptyProductList()
        {
            _mockProductRepo.Setup(repo => repo.GetAll(1, 10, true)).Returns(new ResponseDto { IsSuccess = false });

            var result = await _controller.Index(null, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.Model as List<ProductDto>;

            Assert.NotNull(model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task Index_WithSearchQuery_ReturnsFilteredProducts()
        {
            var mockProducts = new List<Product> { new Product { ProductName = "Test Product 1" }, new Product { ProductName = "Test Product 2" } };
            var mockProductVM = new ProductVM { Products = mockProducts };
            var mockResponse = new ResponseDto { IsSuccess = true, Result = JsonConvert.SerializeObject(mockProductVM) };

            _mockProductRepo.Setup(repo => repo.GetAll(1, 10, true)).Returns(mockResponse);

            var result = await _controller.Index("Test", 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData["lazyLoadData"] as List<Product>; // Accessing ViewData instead of ViewBag

            Assert.NotNull(model);
            Assert.All(model, p => Assert.Contains("Test", p.ProductName));
        }

        [Fact]
        public async Task Create_Get_ReturnsViewResult_WithProductDto()
        {
            _mockTokenProvider.Setup(provider => provider.GetToken()).Returns("mockToken");

            var mockResponse = new ResponseDto
            {
                IsSuccess = true,
                Result = JsonConvert.SerializeObject(new List<CategoryDto>
        {
            new CategoryDto { CategoryId = 1, CategoryName = "Test Category" }
        })
            };

            _mockCategoryRepository.Setup(repo => repo.GetAllCategoriesAsync(1, 100, true)).ReturnsAsync(mockResponse);

            var result = await _controller.Create(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductDto>(viewResult.Model);

            var categories = viewResult.ViewData["ListCategory"] as List<SelectListItem>;

            Assert.NotNull(categories);
            Assert.Single(categories);
            Assert.Equal("Test Category", categories.First().Text);
        }       

        [Fact]
        public async Task Create_Post_ReturnsRedirectToAction_WhenProductIsValidAndCreated()
        {
            var mockFile = new Mock<IFormFile>();
            var productDto = new ProductDto { ProductName = "Test Product", CategoryId = 1, Price = 100 };
            var product = new Product { ProductName = "Test Product", CategoryId = 1, Price = 100 };

            _mockHelperRepository.Setup(repo => repo.UploadImageAsync(mockFile.Object)).ReturnsAsync(new ResponseDto { Result = "mockImageUrl.jpg" });
            _mockProductRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Returns(new ResponseDto { IsSuccess = true });

            var result = await _controller.Create(productDto, mockFile.Object);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Get_ReturnsViewResult_WithCorrectProduct()
        {
            var mockProductDto = new ProductDto { ProductId = 1, ProductName = "Test Product" };
            var mockCategoryDtoList = new List<CategoryDto> { new CategoryDto { CategoryId = 1, CategoryName = "Test Category" } };
            var mockCategoryResponse = new ResponseDto { IsSuccess = true, Result = JsonConvert.SerializeObject(mockCategoryDtoList) };
            var mockResponse = new ResponseDto { IsSuccess = true, Result = JsonConvert.SerializeObject(mockProductDto) };

            _mockCategoryRepository.Setup(repo => repo.GetAllCategoriesAsync(It.IsAny<int>(), It.IsAny<int>(), true))
                             .ReturnsAsync(mockCategoryResponse);
            _mockProductRepo.Setup(repo => repo.GetById(1)).Returns(mockResponse);

            var result = await _controller.Edit(1, 1, 10);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductDto>(viewResult.Model);

            Assert.True(viewResult.ViewData.ContainsKey("ListCategory"), "ListCategory was not found in ViewData.");
            var listCategory = viewResult.ViewData["ListCategory"] as List<SelectListItem>;

            Assert.NotNull(listCategory);
            Assert.Single(listCategory);
            Assert.Equal("Test Category", listCategory[0].Text);
            Assert.Equal(1, model.ProductId);
            Assert.Equal("Test Product", model.ProductName);
        }

        [Fact]
        public async Task Edit_Post_ReturnsRedirectToAction_WhenProductIsUpdated()
        {
            var productDto = new ProductDto { ProductId = 1, ProductName = "Updated Product", CategoryId = 1, Price = 100 };

            _mockProductRepo.Setup(repo => repo.Update(productDto)).Returns(new ResponseDto { IsSuccess = true });

            var result = await _controller.Edit(productDto, productDto.ProductId, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void Delete_ReturnsRedirectToAction_WhenProductIsDeleted()
        {
            var mockResponse = new ResponseDto { IsSuccess = true };

            _mockProductRepo.Setup(repo => repo.Delete(1)).Returns(mockResponse);

            var result = _controller.Delete(1, null, 1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void Details_ReturnsViewResult_WithProductDto()
        {
            var mockProductDto = new ProductDto { ProductId = 1, ProductName = "Test Product" };
            var mockResponse = new ResponseDto { IsSuccess = true, Result = JsonConvert.SerializeObject(mockProductDto) };

            _mockProductRepo.Setup(repo => repo.GetById(1)).Returns(mockResponse);

            var result = _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductDto>(viewResult.Model);

            Assert.Equal(1, model.ProductId);
            Assert.Equal("Test Product", model.ProductName);
        }
    }
}
