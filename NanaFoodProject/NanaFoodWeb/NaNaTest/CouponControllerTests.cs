using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NanaFoodApi.Controllers;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;

namespace NaNaTest
{
    public class CouponControllerTests
    {
        private Mock<ICouponRepo> _couponRepoMock;
        private Mock<IUserCouponRepo> _userCouponRepoMock;
        private Mock<IMapper> _mapperMock;
        private Mock<SignInManager<User>> _signInManagerMock;
        private CouponController _controller;

        public void Setup()
        {
            _couponRepoMock = new Mock<ICouponRepo>();
            _userCouponRepoMock = new Mock<IUserCouponRepo>();
            _mapperMock = new Mock<IMapper>();
            _signInManagerMock = new Mock<SignInManager<User>>(
                new Mock<UserManager<User>>().Object, null, null, null, null, null, null, null, null
            );
            _controller = new CouponController(_couponRepoMock.Object, _mapperMock.Object, _signInManagerMock.Object, _userCouponRepoMock.Object);
        }

        [Fact]
        public async Task CreatCoupon_ValidData_ReturnOk()
        {
            var couponDto = new CouponDto { CouponCode = "SALE25", Discount = 25000 };
            var coupon = new Coupon { CouponCode = "SALE25", Discount = 25000 };

            _mapperMock.Setup(m => m.Map<Coupon>(couponDto)).Returns(coupon);
            _couponRepoMock.Setup(repo => repo.Create(It.IsAny<Coupon>())).ReturnsAsync(new ResponseDto { IsSuccess = true });

            var result = await _controller.CreatCoupon(couponDto) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task CreatCoupon_InvalidData_ReturnBadRequest()
        {
            _controller.ModelState.AddModelError("CouponCode", "Required");

            var result = await _controller.CreatCoupon(new CouponDto()) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task GetAll_ExistingCoupons_ReturnOk()
        {
            _couponRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(new ResponseDto { IsSuccess = true });

            var result = await _controller.GetAll() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task Update_ValidCoupon_ReturnOk()
        {
            var couponDto = new CouponDto { CouponCode = "SALE25", Discount = 25000 };
            var coupon = new Coupon { CouponCode = "SALE25", Discount = 25000 };

            _mapperMock.Setup(m => m.Map<Coupon>(couponDto)).Returns(coupon);
            _couponRepoMock.Setup(repo => repo.Update(It.IsAny<Coupon>())).ReturnsAsync(new ResponseDto { IsSuccess = true });

            var result = await _controller.Update(couponDto) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task DeleteCoupon_ValidCode_ReturnOk()
        {
            _couponRepoMock.Setup(repo => repo.DeleteById("SALE25"))
                           .ReturnsAsync(new ResponseDto { IsSuccess = true });

            var result = await _controller.DeleteCoupon("SALE25");
            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ValidCode_ReturnOk()
        {
            _couponRepoMock.Setup(repo => repo.GetById("SALE25")).ReturnsAsync(new ResponseDto { IsSuccess = true });

            var result = await _controller.GetById("SALE25") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task Check_ValidCoupon_ReturnOk()
        {
            _signInManagerMock.Setup(sm => sm.UserManager.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user123");
            _userCouponRepoMock.Setup(repo => repo.ApplyCoupon("user123", "SALE25")).ReturnsAsync(new ResponseDto { IsSuccess = true });

            var result = await _controller.Check("SALE25") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}