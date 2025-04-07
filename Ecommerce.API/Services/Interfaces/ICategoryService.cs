using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto?> GetCategoryById(Guid categoryId, string? includeProperties = null);
        void AddCategory(CategoryDto category);
    }
}
