using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto?> GetReviewByIdAsync(Guid id);
        Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(Guid productId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(Guid userId);
        Task<ReviewDto?> CreateReviewAsync(CreateReviewDto createReviewDto);
        Task<ReviewDto?> UpdateReviewAsync(Guid id, UpdateReviewDto updateReviewDto);
        Task<bool> DeleteReviewAsync(Guid id);
        //Task<bool> UserCanReviewProductAsync(Guid userId, Guid productId);
        Task<bool> IsOrderCompletedForProductAsync(Guid userId, Guid productId);
        Task<double> GetAverageRatingByProductId(Guid productId);
        Task<int> GetTotalReviewsByProductId(Guid productId);
    }
}
