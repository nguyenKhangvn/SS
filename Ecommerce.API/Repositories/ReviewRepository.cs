using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly EcommerceDbContext _context;

        public ReviewRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        //public async Task<bool> HasUserReviewedProductAsync(Guid userId, Guid productId)
        //{
        //    return await _context.Reviews
        //        .AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        //}

        public async Task<bool> IsOrderCompletedForProductAsync(Guid userId, Guid productId)
        {
            // Check if the user has completed an order containing this product
            return await _context.Orders
                .Include(o => o.OrderItems)
                .AnyAsync(o =>
                    o.UserId == userId &&
                    (o.Status == OrderStatus.COMPLETED || o.Status == OrderStatus.DELIVERED) &&
                    o.OrderItems.Any(oi => oi.ProductId == productId));
        }

        public async Task<Review> CreateAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == review.Id) ?? review;
        }

        public async Task<Review?> UpdateAsync(Review review)
        {
            var existingReview = await _context.Reviews.FindAsync(review.Id);

            if (existingReview == null)
                return null;

            // Only update specific fields
            existingReview.Stars = review.Stars;
            existingReview.Comment = review.Comment;
            existingReview.ImageUrl = review.ImageUrl;
            existingReview.VideoUrl = review.VideoUrl;
            existingReview.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingReview;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetAverageRatingByProductIdAsync(Guid productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (!reviews.Any())
            {
                return 0;
            }

            return reviews.Average(r => r.Stars);
        }

        public async Task<int> GetTotalReviewsByProductIdAsync(Guid productId)
        {
            return await _context.Reviews
                .CountAsync(r => r.ProductId == productId);
        }
        //public async Task<User?> GetUserByIdAsync(Guid userId)
        //{
        //    return await _context.Users.FindAsync(userId);
        //}
    }

}
