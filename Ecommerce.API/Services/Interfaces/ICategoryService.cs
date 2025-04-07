using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, string? includeProperties = null);
        void AddCategory(CreateCategoryDto category);
        Task<CreateCategoryDto> UpdateCategory(CreateCategoryDto category);
        Task<bool> DeleteCategory(Guid categoryId);
        IEnumerable<CategoryDto> GetCategories();
    }
}
