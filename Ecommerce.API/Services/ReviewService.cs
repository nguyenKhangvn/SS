using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, IImageService imageService)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(Guid id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            return review != null ? _mapper.Map<ReviewDto>(review) : null;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(Guid productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(Guid userId)
        {
            var reviews = await _reviewRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> CreateReviewAsync([FromForm] CreateReviewDto createReviewDto)
        {
            // Check if the user has already reviewed this product
            bool hasReviewed = await _reviewRepository.IsOrderCompletedForProductAsync(createReviewDto.UserId, createReviewDto.ProductId);
            if (!hasReviewed)
            {
                return null; // User has already reviewed this product
            }

            //var user = _reviewRepository.GetUserByIdAsync(createReviewDto.UserId);
            //if (user == null)
            //{
            //    Console.WriteLine($"User {createReviewDto.UserId} not found");
            //    return null;
            //}
            if (createReviewDto.ImageFile != null)
            {
                var imageUrl = await _imageService.UpdateImageToCloudAsync(createReviewDto.ImageFile);
                var image1 = new Review
                {
                    ProductId = createReviewDto.ProductId,
                    ImageUrl = imageUrl,
                    UserId = createReviewDto.UserId,
                    Stars = createReviewDto.Stars,
                    Comment = createReviewDto.Comment,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                var createdReview = await _reviewRepository.CreateAsync(image1);
                return _mapper.Map<ReviewDto>(createdReview);
            }
            //var review = _mapper.Map<Review>(createReviewDto);
            var image = new Review
            {
                ProductId = createReviewDto.ProductId,
                ImageUrl = "",
                UserId = createReviewDto.UserId,
                Stars = createReviewDto.Stars,
                Comment = createReviewDto.Comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var createdReview1 = await _reviewRepository.CreateAsync(image);
            return _mapper.Map<ReviewDto>(createdReview1);
        }

        public async Task<ReviewDto?> UpdateReviewAsync(Guid id, UpdateReviewDto updateReviewDto)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(id);

            if (existingReview == null)
            {
                return null;
            }

            // Map update DTO to entity
            _mapper.Map(updateReviewDto, existingReview);

            var updatedReview = await _reviewRepository.UpdateAsync(existingReview);
            return updatedReview != null ? _mapper.Map<ReviewDto>(updatedReview) : null;
        }

        public async Task<bool> DeleteReviewAsync(Guid id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);

            if (review == null)
            {
                return false;
            }

            return await _reviewRepository.DeleteAsync(id);
        }

        //public async Task<bool> UserCanReviewProductAsync(Guid userId, Guid productId)
        //{
        //    // A user can only review a product if they have a completed order containing that product
        //    return await _reviewRepository.IsOrderCompletedForProductAsync(userId, productId);
        //}

        public Task<double> GetAverageRatingByProductIdAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalReviewsByProductIdAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public async Task<double> GetAverageRatingByProductId(Guid productId)
        {
            return await _reviewRepository.GetAverageRatingByProductIdAsync(productId);
        }

        public async Task<int> GetTotalReviewsByProductId(Guid productId)
        {
            return await _reviewRepository.GetTotalReviewsByProductIdAsync(productId);
        }

        public async Task<bool> IsOrderCompletedForProductAsync(Guid userId, Guid productId)
        {
            return await _reviewRepository.IsOrderCompletedForProductAsync(userId, productId);
        }


    }

}
