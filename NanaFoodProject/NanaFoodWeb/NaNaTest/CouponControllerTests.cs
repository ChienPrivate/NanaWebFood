using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NanaFoodApi.Controllers;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;
using Xunit;

namespace NaNaTest
{
    public class CouponControllerTests
    {
        private readonly Mock<ICouponRepo> _couponRepoMock;
        private readonly Mock<IUserCouponRepo> _userCouponRepoMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CouponController _controller;

        public CouponControllerTests()
        {
            _couponRepoMock = new Mock<ICouponRepo>();
            _userCouponRepoMock = new Mock<IUserCouponRepo>();
            //_signInManagerMock = new Mock<SignInManager<User>>(Mock.Of<UserManager<User>>(), null, null, null, null, null, null, null, null);
            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<User>>(userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);

            _mapperMock = new Mock<IMapper>();

            _controller = new CouponController(_couponRepoMock.Object, _mapperMock.Object, _signInManagerMock.Object, _userCouponRepoMock.Object);
        }

        [Fact]
        public async Task CreatCoupon_ValidData_ReturnOkResult()
        {
            
            var dto = new CouponDto { CouponCode = "SALE25", Discount = 25000 };
            var coupon = new Coupon { CouponCode = "SALE25", Discount = 25000 };

            _mapperMock.Setup(m => m.Map<Coupon>(dto)).Returns(coupon);
            _couponRepoMock.Setup(repo => repo.Create(It.IsAny<Coupon>())).ReturnsAsync(new ResponseDto { IsSuccess = true });

            
            var result = await _controller.CreatCoupon(dto);

            
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreatCoupon_InvalidData_ReturnBadRequest()
        {
            
            _controller.ModelState.AddModelError("Error", "Invalid model state");

            
            var result = await _controller.CreatCoupon(new CouponDto());

            
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAll_ReturnOkResult_WithCouponList()
        {
            
            _couponRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(new ResponseDto { IsSuccess = true, Result = new List<CouponDto>() });

            
            var result = await _controller.GetAll();

            
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAll_WhenErrorOccurs_ReturnBadRequest()
        {
            
            _couponRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(new ResponseDto { IsSuccess = false });

            
            var result = await _controller.GetAll();

            
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ValidData_ReturnOkResult()
        {
            
            var dto = new CouponDto { CouponCode = "SALE25", Discount = 25000 };
            var coupon = new Coupon { CouponCode = "SALE25", Discount = 25000 };

            _mapperMock.Setup(m => m.Map<Coupon>(dto)).Returns(coupon);
            _couponRepoMock.Setup(repo => repo.Update(It.IsAny<Coupon>())).ReturnsAsync(new ResponseDto { IsSuccess = true });

            
            var result = await _controller.Update(dto);

            
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_InvalidData_ReturnBadRequest()
        {
            
            _controller.ModelState.AddModelError("Error", "Invalid model state");

            
            var result = await _controller.Update(new CouponDto());

            
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCoupon_ExistingCoupon_ReturnOkResult()
        {
            
            _couponRepoMock.Setup(repo => repo.DeleteById("SALE25")).ReturnsAsync(new ResponseDto { IsSuccess = true });

            
            var result = await _controller.DeleteCoupon("SALE25");

            //Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
        }       

        [Fact]
        public async Task GetById_ExistingCoupon_ReturnOkResult()
        {
            
            _couponRepoMock.Setup(repo => repo.GetById("SALE25")).ReturnsAsync(new ResponseDto { IsSuccess = true, Result = new CouponDto() });

            
            var result = await _controller.GetById("SALE25");

            
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_NonExistentCoupon_ReturnNotFound()
        {
            
            _couponRepoMock.Setup(repo => repo.GetById("NONEXIST")).ReturnsAsync(new ResponseDto { IsSuccess = false });

            
            var result = await _controller.GetById("NONEXIST");

            
            Assert.IsType<NotFoundObjectResult>(result);
        }
       
    }
}
