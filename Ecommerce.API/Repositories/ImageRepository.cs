
namespace Ecommerce.API.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly EcommerceDbContext _context;
        public ImageRepository(EcommerceDbContext context)
        {
            _context = context;
        }
        public async Task<Image> AddImageAsync(Image image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public Task<bool> DeleteImageAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Image>> GetAllImagesAsync()
        {
            return await _context.Images.ToListAsync();
        }

        public Task<Image> GetImageByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Image>> GetImagesByProductIdAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<Image> UpdateImageAsync(Image image)
        {
            throw new NotImplementedException();
        }
    }
}
