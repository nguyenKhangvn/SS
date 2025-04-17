using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IImageService
    {
        Task<ImageDto> GetImageByIdAsync(Guid id);
        Task<List<ImageDto>> GetImagesByProductIdAsync(Guid productId);
        Task<ImageDto> AddImageAsync(ImageDto image);
        Task<string> UpdateImageAsync(IFormFile image);
        Task<bool> DeleteImageAsync(Guid id);
        Task<List<ImageDto>> GetAllImagesAsync();
    }
}
