using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(Guid id);
        Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId);
        //Task<bool> HasUserReviewedProductAsync(Guid userId, Guid productId);
        Task<bool> IsOrderCompletedForProductAsync(Guid userId, Guid productId);
        Task<Review> CreateAsync(Review review);
        Task<Review?> UpdateAsync(Review review);
        Task<bool> DeleteAsync(Guid id);
        Task<double> GetAverageRatingByProductIdAsync(Guid productId);
        Task<int> GetTotalReviewsByProductIdAsync(Guid productId);
        //Task<User?> GetUserByIdAsync(Guid userId);
        
    }
}
