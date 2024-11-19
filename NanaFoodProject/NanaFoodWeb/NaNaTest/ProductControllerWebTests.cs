using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NanaFoodWeb.Controllers;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;

namespace NaNaTest
{
    public class ProductsControllerWebTests
    {
        private readonly Mock<IProductRepo> _productRepoMock = new();
        private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
        private readonly Mock<IReviewRepository> _reviewRepoMock = new();
        private readonly Mock<IHelperRepository> _helperRepoMock = new();
        private readonly Mock<ITokenProvider> _tokenProviderMock = new();
        private readonly ProductsController _controller;

        public ProductsControllerWebTests()
        {
            _controller = new ProductsController(
                _productRepoMock.Object,
                _helperRepoMock.Object,
                _tokenProviderMock.Object,
                _categoryRepoMock.Object,
                _reviewRepoMock.Object
            );
        }

        [Fact]
        public async Task Index_ReturnViewWithProducts_WhenApiResponseIsSuccess()
        {
            // Arrange
            var productList = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Product 1" },
                new Product { ProductId = 2, ProductName = "Product 2" }
            };

            var apiResponse = new ResponseDto
            {
                IsSuccess = true,
                Result = JsonConvert.SerializeObject(productList)
            };

            _productRepoMock
                .Setup(repo => repo.GetProduct())
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var lazyLoadData = viewResult.ViewData["lazyLoadData"] as List<Product>;

            Assert.NotNull(lazyLoadData);
            Assert.Equal(productList.Count, lazyLoadData.Count);
            Assert.Equal(productList[0].ProductName, lazyLoadData[0].ProductName);
        }

        //[Fact]
        //public async Task Create_Get_ReturnViewWithCategories()
        //{
        //    // Arrange
        //    var categories = new List<CategoryDto>
        //    {
        //        new CategoryDto { CategoryId = 1, CategoryName = "Category 1" },
        //        new CategoryDto { CategoryId = 2, CategoryName = "Category 2" }
        //    };

        //    _categoryRepoMock
        //        .Setup(repo => repo.GetAllCategoriesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
        //        .ReturnsAsync(new ResponseDto
        //        {
        //            IsSuccess = true,
        //            Result = JsonConvert.SerializeObject(categories)
        //        });

        //    // Act
        //    var result = await _controller.Create(1, 100);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var listCategory = viewResult.ViewData["ListCategory"] as List<SelectListItem>;

        //    Assert.NotNull(listCategory);
        //    Assert.Equal(2, listCategory.Count);
        //    Assert.Equal("Category 1", listCategory[0].Text);
        //    Assert.Equal("1", listCategory[0].Value);
        //}

        [Fact]
        public async Task Create_Post_ShouldRedirectToIndex_WhenProductIsCreatedSuccessfully()
        {
            // Arrange
            var productDto = new ProductDto { ProductName = "New Product", CategoryId = 1, Price = 100 };
            var apiResponse = new ResponseDto { IsSuccess = true };

            _helperRepoMock.Setup(helper => helper.UploadImageAsync(It.IsAny<IFormFile>()))
                           .ReturnsAsync(new ResponseDto { Result = "image_url" });

            _productRepoMock.Setup(repo => repo.Create(It.IsAny<Product>())).Returns(apiResponse);

            // Act
            var result = await _controller.Create(productDto, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        //[Fact]
        //public async Task Edit_Get_ReturnViewWithProduct_WhenProductExists()
        //{
        //    // Arrange
        //    var productDto = new ProductDto { ProductId = 1, ProductName = "Product 1" };
        //    var apiResponse = new ResponseDto
        //    {
        //        IsSuccess = true,
        //        Result = JsonConvert.SerializeObject(productDto)
        //    };

        //    _productRepoMock.Setup(x => x.GetProduct())
        //                   .ReturnsAsync(new ResponseDto());

        //    // Act
        //    var result = await _controller.Edit(1, 1, 10);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<ProductDto>(viewResult.Model);

        //    Assert.NotNull(model);
        //    Assert.Equal(1, model.ProductId);
        //    Assert.Equal("Product 1", model.ProductName);
        //}

        //[Fact]
        //public void DeleteConfirm_RedirectToIndex_WhenDeleteIsSuccessful()
        //{
        //    // Arrange
        //    var apiResponse = new ResponseDto { IsSuccess = true };

        //    _productRepoMock
        //        .Setup(repo => repo.Delete(It.IsAny<int>()))
        //        .Returns(apiResponse);

        //    // Act
        //    var result = _controller.DeleteConfirm(1);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        //    Assert.Equal("Index", redirectResult.ActionName);
        //    _productRepoMock.Verify(repo => repo.Delete(1), Times.Once);
        //}

    }
}