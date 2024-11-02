using AutoMapper;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Helper
{
    internal class MapConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingconfig = new MapperConfiguration(config =>
            {
                // Viết Model muốn mapping với DTO vào đây 
                // VD : config.CreateMap<ProductDto, Products>().ReverseMap();
                config.CreateMap<CategoryDto, Category>().ReverseMap();
                config.CreateMap<ProductDto, Product>().ReverseMap();
                config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
                config.CreateMap<OrderDto, Order>().ReverseMap();
                config.CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();
                config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
                config.CreateMap<WishListDto,WishList>().ReverseMap();
                config.CreateMap<ProductChangeLogDto,ProductChangeLog>().ReverseMap();
                config.CreateMap<SearchHistoryDto,SearchHistory>().ReverseMap();
                config.CreateMap<UserDto, User>().ReverseMap();
                config.CreateMap<CreateUserRequestDto, User>().ReverseMap();
                config.CreateMap<UpdateUserRequestDto, User>().ReverseMap();
                config.CreateMap<ReviewDto, Review>().ReverseMap();
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
                config.CreateMap<UserCouponDto, UserCoupon>().ReverseMap();
            });
            return mappingconfig;
        }
    }
}
