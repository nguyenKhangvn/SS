using Ecommerce.API.Repositories;
using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Dtos;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        public void AddCategory(CategoryDto model)
        {
            _categoryRepository.AddCategory(new Category
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description
            });
        }

        public async Task<CategoryDto?> GetCategoryById(Guid categoryId, string? includeProperties)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId, includeProperties);

            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

    }
}
