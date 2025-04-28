using Ecommerce.API.Hubs;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatParticipantRepository _participantRepository;
        private readonly IUserRepository _userRepository;

        public ChatService(
            IChatRepository chatRepository,
            IMessageRepository messageRepository,
            IChatParticipantRepository participantRepository,
            IUserRepository userRepository) // Inject UserRepository
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _participantRepository = participantRepository;
            _userRepository = userRepository;
        }

        public async Task<ChatDto?> GetChatByIdAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null)
            {
                return null;
            }
            return MapChatToDto(chat);
        }

        public async Task<IEnumerable<ChatDto>> GetChatsForUserAsync(Guid userId)
        {
            var chats = await _chatRepository.GetChatsForUserAsync(userId);
            return chats.Select(chat => MapChatToDto(chat));
        }

        public async Task<ChatDto> CreateChatAsync(CreateChatRequest request, Guid creatorUserId)
        {
            // Validate if participant user IDs exist (optional but recommended)
            var participantUsers = await _userRepository.(request.ParticipantUserIds.Concat(new[] { creatorUserId }).Distinct().ToList());
            if (participantUsers.Count() != request.ParticipantUserIds.Concat(new[] { creatorUserId }).Distinct().Count())
            {
                // Handle case where one or more participant user IDs are invalid
                throw new ArgumentException("One or more participant user IDs are invalid.");
            }


            var chat = new Chat
            {
                Title = request.Title,
                Status = ChatStatus.ACTIVE, // Assuming ChatStatus is an enum
                // BaseEntity properties like CreatedAt, UpdatedAt will be set by EF Core or BaseEntity logic
            };

            await _chatRepository.AddAsync(chat);

            // Add participants
            foreach (var userId in request.ParticipantUserIds.Concat(new[] { creatorUserId }).Distinct())
            {
                var participant = new ChatParticipant
                {
                    ChatId = chat.Id,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow // Or use BaseEntity's CreatedAt
                };
                await _participantRepository.AddAsync(participant);
            }

            // Reload chat with participants to map correctly
            var createdChat = await _chatRepository.GetByIdAsync(chat.Id);

            return MapChatToDto(createdChat);
        }

        public async Task<MessageDto?> SendMessageAsync(SendMessageRequest request, Guid senderUserId)
        {
            // Check if chat exists
            var chatExists = await _chatRepository.ExistsAsync(request.ChatId);
            if (!chatExists)
            {
                return null; // Or throw an exception
            }

            // Check if sender is a participant of the chat
            var isParticipant = await _participantRepository.IsUserInChatAsync(request.ChatId, senderUserId);
            if (!isParticipant)
            {
                // User is not in the chat, cannot send message
                return null; // Or throw an exception
            }

            var message = new Message
            {
                ChatId = request.ChatId,
                SenderId = senderUserId,
                Content = request.Content,
                SentAt = DateTime.UtcNow // Or use BaseEntity's CreatedAt
            };

            await _messageRepository.AddAsync(message);

            // Fetch the sender's name to include in the DTO
            var sender = await _userRepository.GetByIdAsync(senderUserId);
            if (sender == null)
            {
                // This case should ideally not happen if IsUserInChatAsync passed,
                // but handle defensively.
                return null;
            }

            return MapMessageToDto(message, sender.Name);
        }

        public async Task<bool> JoinChatAsync(JoinChatRequest request)
        {
            var chatExists = await _chatRepository.ExistsAsync(request.ChatId);
            if (!chatExists)
            {
                return false; 
            }

            // Check if user already in chat
            var isParticipant = await _participantRepository.IsUserInChatAsync(request.ChatId, request.UserId);
            if (isParticipant)
            {
                return false; // User is already a participant
            }

            var participant = new ChatParticipant
            {
                ChatId = request.ChatId,
                UserId = request.UserId,
                JoinedAt = DateTime.UtcNow // Or use BaseEntity's CreatedAt
            };

            await _participantRepository.AddAsync(participant);
            return true;
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesForChatAsync(Guid chatId, Guid userId, int skip = 0, int take = 50)
        {
            var isParticipant = await _participantRepository.IsUserInChatAsync(chatId, userId);
            if (!isParticipant)
            {
                return Enumerable.Empty<MessageDto>(); 
            }

            var messages = await _messageRepository.GetMessagesForChatAsync(chatId, skip, take);

            var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
            var senders = (await _userRepository.GetByIdsAsync(senderIds)).ToDictionary(u => u.Id, u => u.Name);

            return messages.Select(m => MapMessageToDto(m, senders.GetValueOrDefault(m.SenderId, "Unknown User")));
        }

        // Helper methods to map Entities to DTOs
        private ChatDto MapChatToDto(Chat chat)
        {
            return new ChatDto
            {
                Id = chat.Id,
                Title = chat.Title,
                Status = chat.Status.ToString(), // Assuming ChatStatus is an enum
                CreatedAt = chat.CreatedAt, // Assuming BaseEntity has these properties
                UpdatedAt = chat.UpdatedAt, // Assuming BaseEntity has these properties
                Messages = chat.Messages.Select(m => MapMessageToDto(m, m.Sender?.Name ?? "Unknown User")).ToList(), // Map messages
                Participants = chat.Participants.Select(cp => MapChatParticipantToDto(cp)).ToList() // Map participants
            };
        }

        private MessageDto MapMessageToDto(Message message, string senderName)
        {
            return new MessageDto
            {
                Id = message.Id,
                ChatId = message.ChatId,
                SenderId = message.SenderId,
                SenderName = senderName,
                Content = message.Content,
                SentAt = message.SentAt
            };
        }

        private ChatParticipantDto MapChatParticipantToDto(ChatParticipant participant)
        {
            return new ChatParticipantDto
            {
                UserId = participant.UserId,
                UserName = participant.User?.Name ?? "Unknown User", // Assuming User has a Name property
                JoinedAt = participant.JoinedAt
            };
        }
    }
} 