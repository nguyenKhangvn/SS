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
            //// Create mappings between DTOs and entities
            //CreateMap<ProductDto, Product>().ReverseMap();
            CreateMap<CategoryDto, Category>().ReverseMap();
            //CreateMap<ManufacturerDto, Manufacturer>().ReverseMap();
            //CreateMap<DiscountDto, Discount>().ReverseMap();
            // Add other mappings as needed
        }
    }
}
