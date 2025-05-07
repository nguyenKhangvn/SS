using AutoMapper;
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



        public async Task<bool> DeleteCategory(Guid categoryId)
        {
            var result = await _categoryRepository.DeleteCategory(categoryId);
            return result;
        }

        public IEnumerable<CategoryDto> GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            return categories.Select(c => _mapper.Map<CategoryDto>(c));
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, string? includeProperties)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId, includeProperties);
            return _mapper.Map<CategoryDto>(category);
        }


        public async Task<CreateCategoryDto> UpdateCategory(CreateCategoryDto category)
        {
            var categoryToUpdate = _mapper.Map<Category>(category);
            var res = await _categoryRepository.UpdateCategory(categoryToUpdate);
            return _mapper.Map<CreateCategoryDto>(res);
        }
    }
}
