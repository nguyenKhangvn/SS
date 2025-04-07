
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
            var category = new Category
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
               
                //Products = model.Products,
                //SubCategories = model.SubCategories

            };


            _service.Categories.Add(category);
            _service.SaveChanges();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId, string? includeProperties = null)
        {
            IQueryable<Category> query = _service.Categories;

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(c => c.Id == categoryId);
        }
    }
}
