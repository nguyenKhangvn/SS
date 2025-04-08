using AutoMapper;
using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            CreateMap<StoreLocationDto, StoreLocation>().ReverseMap();
            CreateMap<PostDto, Post>().ReverseMap();
            CreateMap<UserCreateDto, User>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<ManufacturerDto, Manufacturer>().ReverseMap();
            CreateMap<AddressDto, Address>().ReverseMap();
        }
    }
}
