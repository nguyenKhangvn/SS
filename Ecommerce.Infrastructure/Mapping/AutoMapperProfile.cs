﻿using AutoMapper;
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
            //post
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
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.StoreInventories.Sum(si => si.Quantity)))
                .ForMember(dest => dest.Sold, opt => opt.MapFrom(src => src.StoreInventories.Sum(si => si.Sold)))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.StoreInventories.FirstOrDefault().StoreLocation.Name));
            // .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            //.ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer.Name))
            //.ForMember(dest => dest.DiscountName, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.Name : null));
            CreateMap<ProductDto, Product>();

            CreateMap<ProductCreateDto, Product>().ReverseMap();
            CreateMap<ProductUpdateDto, Product>().ReverseMap();

            // order item
            CreateMap<OrderItemDto, OrderItem>().ReverseMap();

            //image
            CreateMap<Image, ImageDto>().ReverseMap();

            //product store inventory
            CreateMap<ProductStoreInventoryDto, ProductStoreInventory>();
            CreateMap<ProductStoreInventory, ProductStoreInventoryDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.StoreLocation.Name));

            CreateMap<AddOrUpdateProductStoreInventoryDto, ProductStoreInventory>().ReverseMap();

            // payment
            CreateMap<PaymentDto, Payment>().ReverseMap();

            // review
            CreateMap<ReviewDto, Review>().ReverseMap();
            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateReviewDto, Review>().ReverseMap();
            //chat
            CreateMap<ChatDto, Chat>().ReverseMap();
            CreateMap<ChatMessageDto, Message>().ReverseMap();
            CreateMap<ChatParticipantDto, ChatParticipant>().ReverseMap();

            // coupon 
            CreateMap<CouponDto, Coupon>().ReverseMap();
            CreateMap<CouponCreateDto, Coupon>().ReverseMap();

            //message
            CreateMap<MessageDto, Message>().ReverseMap();        
        }
    }
}