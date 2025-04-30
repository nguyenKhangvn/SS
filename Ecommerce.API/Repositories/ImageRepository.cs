
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

        public async Task<bool> DeleteImageAsync(Guid id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
                return false;

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Image>> GetAllImagesAsync()
        {
            return await _context.Images.ToListAsync();
        }

        public async Task<Image> GetImageByIdAsync(Guid id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                throw new InvalidOperationException($"Image with ID {id} not found.");
            }
            return image;
        }

        public async Task<List<Image>> GetImagesByProductIdAsync(Guid productId)
        {
            return await _context.Images
                         .Where(i => i.ProductId == productId)
                         .ToListAsync();
        }

        public async Task<Image> UpdateImageAsync(Image image)
        {
            var existingImage = await _context.Images.FindAsync(image.Id);
            if (existingImage == null)
                return null;

            existingImage.ProductId = image.ProductId;
            existingImage.Url = image.Url;

            _context.Images.Update(existingImage);
            await _context.SaveChangesAsync();
            return existingImage;
        }
    }
}
