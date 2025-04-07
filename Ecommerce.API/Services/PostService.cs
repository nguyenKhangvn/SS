using AutoMapper;
using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }
        public async Task<PostDto> AddPostAsync(PostDto post)
        {
            var postEntity = _mapper.Map<Post>(post);
            var createdPost = await _postRepository.AddPostAsync(postEntity);
            return _mapper.Map<PostDto>(createdPost);
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await _postRepository.GetPostByIdAsync(postId);
            if (post == null)
            {
                return false;
            }
            return await _postRepository.DeletePostAsync(postId);
        }

        public async Task<PostDto?> GetPostByIdAsync(Guid postId, string? includeProperties = null)
        {
            var post = await _postRepository.GetPostByIdAsync(postId, includeProperties);
            if (post == null)
            {
                return null;
            }
            return _mapper.Map<PostDto>(post);
        }

        public async Task<IEnumerable<PostDto>> GetPostsAsync(string? includeProperties = null)
        {
            var posts = await _postRepository.GetPostsAsync(includeProperties);
            return posts.Select(p => _mapper.Map<PostDto>(p));
        }

        public async Task<PostDto> UpdatePostAsync(PostDto post)
        {
            var postEntity = _mapper.Map<Post>(post);
            var updatedPost = await _postRepository.UpdatePostAsync(postEntity);
            return _mapper.Map<PostDto>(updatedPost);
        }
    }
}
