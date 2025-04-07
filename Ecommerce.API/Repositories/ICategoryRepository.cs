using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetCategoryByIdAsync(Guid categoryId, string? includeProperties = null);
        void AddCategory(Category category);
    }
}
