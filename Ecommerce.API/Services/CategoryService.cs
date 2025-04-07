using AutoMapper;
using Ecommerce.API.Repositories;
using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Dtos;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace Ecommerce.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }


        public void AddCategory(CreateCategoryDto model)
        {
            var category = _mapper.Map<Category>(model);
            _categoryRepository.AddCategory(category);
        }



        public bool DeleteCategory(CategoryDto category)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CategoryDto> GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            return categories.Select(c => _mapper.Map<CategoryDto>(c));
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, string? includeProperties)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId, includeProperties);

            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name,
                ProductCount = category.Products?.Count ?? 0,
                SubCategoryCount = category.SubCategories?.Count ?? 0
            };
        }


        public async Task<CreateCategoryDto> UpdateCategory(CreateCategoryDto category)
        {
            var categoryToUpdate = _mapper.Map<Category>(category);
            var res = await _categoryRepository.UpdateCategory(categoryToUpdate);

            if ( res == null)
            {
                throw new Exception("sai");
            }
            return _mapper.Map<CreateCategoryDto>(res);
        }
    }
}
