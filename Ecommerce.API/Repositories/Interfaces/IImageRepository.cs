namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IImageRepository
    {
        Task<Image> AddImageAsync(Image image);
        Task<bool> DeleteImageAsync(Guid id);
        Task<Image> GetImageByIdAsync(Guid id);
        Task<List<Image>> GetImagesByProductIdAsync(Guid productId);
        Task<Image> UpdateImageAsync(Image image);

        //get all
        Task<List<Image>> GetAllImagesAsync();
    }
}
