using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IChatService
    {
        Task<ChatDto?> GetChatByIdAsync(Guid chatId);
        Task<IEnumerable<ChatDto>> GetChatsForUserAsync(Guid userId);
        Task<bool> JoinChatAsync(JoinChatRequest request);
        Task<ChatMessageDto> SendMessageAsync(Guid senderId, SendMessageDto message);
        Task<IEnumerable<ChatConversationDto>> GetUserConversationsAsync(Guid userId);
        Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId);
        Task<ChatDto> CreateStaffChatAsync(Guid customerId);
        Task<IEnumerable<ChatMessageDto>> GetMessagesByChatIdAsync(Guid chatId);
        Task<Guid> GetOrCreateChatWithUserAsync(Guid customerId);
        //Task<ChatMessageDto?> SendMessageAsync(Guid senderUserId, SendMessageRequest request);
        //Task<ChatDto> CreateChatAsync(CreateChatRequest request, Guid creatorUserId);
        //Task<int> GetUnreadCountAsync(Guid userId);
        //Task<IEnumerable<ChatMessageDto>> GetMessagesForChatAsync(Guid chatId, Guid currentUserId);
        //Task<IEnumerable<UserDto>> GetStaffMembersAsync();
        //Task<IEnumerable<MessageDto>> GetMessagesForChatAsync(Guid chatId, Guid userId, int skip = 0, int take = 50);
        //Task<ChatDto> FindOrCreateAdminChatAsync(Guid userId, Guid adminUserId);
        //Task<IEnumerable<UserDto>> GetCustomersAsync();
        //Task<ChatMessageDto?> SendMessageToStaffAsync(Guid senderUserId, string messageContent);
    }
} 