using AutoMapper;
using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.Models.Dtos;


namespace Ecommerce.Infrastructure.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // category
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            // store location
            CreateMap<StoreLocationDto, StoreLocation>().ReverseMap();
            CreateMap<PostDto, Post>().ReverseMap();
            // user
            CreateMap<UserCreateDto, User>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            // manufacturer
            CreateMap<ManufacturerDto, Manufacturer>().ReverseMap();
            // address
            CreateMap<AddressDto, Address>().ReverseMap();
            // order
            CreateMap<OrderDto, Order>().ReverseMap();
            //order item
            CreateMap<OrderItemDto, OrderItem>();
            CreateMap<Coupon, CouponDto>().ReverseMap();
            //product
            CreateMap<ProductDto, Product>().ReverseMap();
            CreateMap<ProductCreateDto, Product>().ReverseMap();
            CreateMap<ProductUpdateDto, Product>().ReverseMap();

            // order item
            CreateMap<OrderItemDto, OrderItem>().ReverseMap();

        }
    }
}
