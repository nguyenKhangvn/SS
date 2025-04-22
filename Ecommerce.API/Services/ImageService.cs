using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public ImageService(IImageRepository imageRepository, IMapper mapper)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
        }
        public async Task<ImageDto> AddImageAsync(ImageDto file)
        {
           await _imageRepository.AddImageAsync(_mapper.Map<Image>(file));
           return file;
        }

        public Task<bool> DeleteImageAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        //get all images
        public async Task<List<ImageDto>> GetAllImagesAsync()
        {
            var images = await _imageRepository.GetAllImagesAsync();
            return _mapper.Map<List<ImageDto>>(images);
        }

        public Task<ImageDto> GetImageByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ImageDto>> GetImagesByProductIdAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdateImageAsync(IFormFile file)
        {
            if (file.Length == 0)
                throw new ArgumentException("File is empty");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("khong cung loai");
            //tao thu muc
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine("wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{fileName}";
        }
    }
}
