using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetCategoryByIdAsync(Guid categoryId, string? includeProperties = null);
        void AddCategory(Category category);
        Task<Category> UpdateCategory(Category category);
        Task<bool> DeleteCategory(Guid categoryId);
        IEnumerable<Category> GetCategories();

    }
}
