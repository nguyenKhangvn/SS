using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IChatService
    {
        Task<ChatDto?> GetChatByIdAsync(Guid chatId);
        Task<IEnumerable<ChatDto>> GetChatsForUserAsync(Guid userId);
        Task<ChatDto> CreateChatAsync(CreateChatRequest request, Guid creatorUserId);
        Task<MessageDto?> SendMessageAsync(SendMessageRequest request, Guid senderUserId);
        Task<bool> JoinChatAsync(JoinChatRequest request);
        Task<IEnumerable<MessageDto>> GetMessagesForChatAsync(Guid chatId, Guid userId, int skip = 0, int take = 50);
    }
} 