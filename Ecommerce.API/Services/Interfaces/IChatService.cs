using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IChatService
    {
        Task<ChatMessageDto> SendMessageAsync(Guid senderId, SendMessageRequest message);
        Task<IEnumerable<ChatMessageDto>> GetMessagesForChatAsync(Guid chatId, Guid currentUserId);
        Task<IEnumerable<ChatConversationDto>> GetUserConversationsAsync(Guid userId);
        Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId);
        Task<int> GetUnreadCountAsync(Guid userId);
        
        // New methods for staff-customer chat
        Task<IEnumerable<UserDto>> GetStaffMembersAsync();
        Task<IEnumerable<UserDto>> GetCustomersAsync();
        Task<ChatDto> CreateStaffChatAsync(Guid customerId);

        Task<IEnumerable<ChatMessageDto>> GetMessagesByChatIdAsync(Guid chatId);
        Task<Guid> GetOrCreateChatWithUserAsync(Guid customerId);

    }
} 