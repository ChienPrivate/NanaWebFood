using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using NanaFoodApi.Controllers;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;
using AutoMapper;

namespace NaNaTest
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _mockFoodService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductController _controller;

        public ProductsControllerTests()
        {
            _mockFoodService = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ProductController(_mockFoodService.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetAll_ReturnsOkResult_WhenProductsExist()
        {
            var responseDto = new ResponseDto { IsSuccess = true };
            _mockFoodService.Setup(svc => svc.GetAll(1, 10, true)).Returns(responseDto);

            var result = _controller.GetAll(1, 10, true);

            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(responseDto, okResult.Value);
        }

        [Fact]
        public void GetAll_ReturnsNotFound_WhenNoProductsExist()
        {
            var responseDto = new ResponseDto { IsSuccess = false };
            _mockFoodService.Setup(svc => svc.GetAll(1, 10, true)).Returns(responseDto);

            var result = _controller.GetAll(1, 10, true);

            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(responseDto, notFoundResult.Value);
        }

        [Fact]
        public void GetById_ReturnsProduct_WhenProductExists()
        {
            var responseDto = new ResponseDto { IsSuccess = true };
            _mockFoodService.Setup(svc => svc.GetById(1)).Returns(responseDto);

            var result = _controller.GetById(1);

            Assert.Equal(responseDto, result);
        }

        [Fact]
        public void Create_ReturnsResponseDto_WhenModelIsValid()
        {
            var productDto = new ProductDto();
            var product = new Product();
            var responseDto = new ResponseDto { IsSuccess = true };

            _mockMapper.Setup(mapper => mapper.Map<Product>(productDto)).Returns(product);
            _mockFoodService.Setup(svc => svc.Create(product)).Returns(responseDto);

            var result = _controller.Create(productDto);

            Assert.Equal(responseDto, result);
        }

        [Fact]
        public async Task GetByCategoryIdExcludeSameProduct_ReturnsOk_WhenSuccess()
        {
            var responseDto = new ResponseDto { IsSuccess = true };
            _mockFoodService.Setup(svc => svc.GetByCategoryIdExcludeSameProduct(1, 1, 1, 10))
                .ReturnsAsync(responseDto);

            var result = await _controller.GetByCategoryIdExcludeSameProduct(1, 1, 1, 10) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(responseDto, result.Value);
        }

        [Fact]
        public void ActiveProduct_ReturnsOkResult_WhenProductActivatedSuccessfully()
        {
            var response = new ResponseDto { IsSuccess = true };
            _mockFoodService.Setup(svc => svc.ModifyStatus(1, true)).Returns(response);

            var result = _controller.ActiveProduct(1);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public void ActiveProduct_ReturnsBadRequest_WhenProductActivationFails()
        {
            var response = new ResponseDto { IsSuccess = false };
            _mockFoodService.Setup(svc => svc.ModifyStatus(1, true)).Returns(response);

            var result = _controller.ActiveProduct(1);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void UnActiveProduct_ReturnsOkResult_WhenProductDeactivatedSuccessfully()
        {
            var response = new ResponseDto { IsSuccess = true };
            _mockFoodService.Setup(svc => svc.ModifyStatus(1, false)).Returns(response);

            var result = _controller.UnActiveProduct(1);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public void UnActiveProduct_ReturnsBadRequest_WhenProductDeactivationFails()
        {
            var response = new ResponseDto { IsSuccess = false };
            _mockFoodService.Setup(svc => svc.ModifyStatus(1, false)).Returns(response);

            var result = _controller.UnActiveProduct(1);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var productDto = new ProductDto();
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = _controller.Update(productDto);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public void Sorting_ReturnsOkResult_WhenSortingSuccessfully()
        {
            var products = new List<ProductDto>
            {
                new ProductDto { ProductName = "Product A" },
                new ProductDto { ProductName = "Product B" }
            };

            _mockFoodService.Setup(svc => svc.Sorting("price", 1, 10)).Returns(new ResponseDto
            {
                IsSuccess = true,
                Result = products
            });

            var result = _controller.Sorting("price", 1, 10);

            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(products, okResult.Value);
        }

    }
}
