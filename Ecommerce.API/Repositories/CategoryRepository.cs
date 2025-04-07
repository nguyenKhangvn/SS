
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private EcommerceDbContext _service;

        public CategoryRepository(EcommerceDbContext service)
        {
            _service = service;
        }
        public void AddCategory(Category model)
        {
            _service.Categories.Add(model);
            _service.SaveChanges();
        }

        public bool DeleteCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Category> GetCategories()
        {
            return _service.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .Include(c => c.Products)
                .ToList();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId, string? includeProperties = null)
        {
            IQueryable<Category> query = _service.Categories;

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split( ',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<Category> UpdateCategory(Category category)
        {

            _service.Categories.Update(category);
            await _service.SaveChangesAsync();
            return category;
        }
    }
}
