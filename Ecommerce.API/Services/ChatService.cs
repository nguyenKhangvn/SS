
using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatParticipantRepository _participantRepository;
        private readonly IUserRepository _userRepository;
        private readonly EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;

        public ChatService(
            IChatRepository chatRepository,
            IMessageRepository messageRepository,
            IChatParticipantRepository participantRepository,
            IUserRepository userRepository,
            EcommerceDbContext dbContext,
            IMapper mapper)
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _participantRepository = participantRepository;
            _userRepository = userRepository;
            _dbContext = dbContext;
            _mapper = mapper;
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
            var participantUsers = await _userRepository.GetByIdsAsync(request.ParticipantUserIds.Concat(new[] { creatorUserId }).Distinct().ToList());
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

        public async Task<ChatMessageDto?> SendMessageAsync(Guid senderUserId, SendMessageRequest request)
        {
            Guid chatId;

            if (request.ChatId == Guid.Empty)
            {
                var existingChat = await _chatRepository.FindChatByCustomerIdAsync(senderUserId);
                if (existingChat != null)
                {
                    chatId = existingChat.Id;
                }
                else
                {
                    var availableStaff = await _userRepository.FindAvailableStaffAsync();
                    if (availableStaff == null)
                        return null;

                    chatId = Guid.NewGuid();
                    var newChat = new Chat
                    {
                        Id = chatId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _chatRepository.AddAsync(newChat);
                    await _participantRepository.AddParticipantAsync(chatId, senderUserId);
                    await _participantRepository.AddParticipantAsync(chatId, availableStaff.Id);
                }
            }
            else
            {
                chatId = request.ChatId;
                var chatExists = await _chatRepository.ExistsAsync(chatId);
                if (!chatExists)
                    return null;

                //var isParticipant = await _participantRepository.GetParticipantsForChatAsync(chatId);
                //if (!isParticipant)
                //    return null;
            }

            // Save message
            var message = new Message
            {
                ChatId = chatId,
                SenderId = senderUserId,
                Content = request.Content,
                SentAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);

            var sender = await _userRepository.GetByIdAsync(senderUserId);
            if (sender == null)
                return null;

            // Find receiver
            var participants = await _participantRepository.GetParticipantsForChatAsync(chatId);
            var receiver = participants.FirstOrDefault(p => p.UserId != senderUserId);
            var receiverUser = receiver != null ? await _userRepository.GetByIdAsync(receiver.UserId) : null;

            return new ChatMessageDto
            {
                Id = message.Id,
                ChatId = message.ChatId,
                SenderId = message.SenderId,
                SenderName = sender.Name,
                ReceiverId = receiverUser?.Id ?? Guid.Empty,
                ReceiverName = receiverUser?.Name ?? null,
                Content = message.Content,
                SentAt = message.SentAt,
                IsRead = false
            };
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
        public async Task<ChatDto> FindOrCreateAdminChatAsync(Guid userId, Guid adminUserId)
        {
            // First, check if a direct chat between the user and admin already exists
            var existingChat = await _dbContext.Chats
                .Include(c => c.Participants)
                .Where(c => c.Participants.Count == 2 &&
                           c.Participants.Any(p => p.UserId == userId) &&
                           c.Participants.Any(p => p.UserId == adminUserId))
                .FirstOrDefaultAsync();

            if (existingChat != null)
            {
                // Chat already exists, return it
                return await GetChatByIdAsync(existingChat.Id);
            }

            // If no direct chat exists, create a new one
            // Validate that admin user exists
            var adminUser = await _dbContext.Users.FindAsync(adminUserId);
            if (adminUser == null)
            {
                throw new ArgumentException($"Admin user with ID {adminUserId} not found.");
            }

            // Get the current user
            var currentUser = await _dbContext.Users.FindAsync(userId);
            if (currentUser == null)
            {
                throw new ArgumentException($"User with ID {userId} not found.");
            }

            // Create a new chat
            var chat = new Chat
            {
                Title = $"Support Chat",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add both users as participants
            chat.Participants = new List<ChatParticipant>
            {
                new ChatParticipant
                {
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow,
                },
                new ChatParticipant
                {
                    UserId = adminUserId,
                    JoinedAt = DateTime.UtcNow,
                }
            };

            // Save to database
            _dbContext.Chats.Add(chat);
            await _dbContext.SaveChangesAsync();

            // Return the complete chat DTO
            return await GetChatByIdAsync(chat.Id);
        }

        public async Task<ChatMessageDto> SendMessageAsync(Guid senderId, SendMessageDto message)
        {
            var receiver = await _userRepository.GetByIdAsync(message.ReceiverId);
            if (receiver == null)
            {
                throw new ArgumentException($"Receiver with ID {message.ReceiverId} not found.");
            }

            // Get sender info
            var sender = await _userRepository.GetByIdAsync(senderId);
            if (sender == null)
            {
                throw new ArgumentException($"Sender with ID {senderId} not found.");
            }

            // Create and save message
            var chatMessage = new Message
            {
                SenderId = senderId,
                Content = message.Content,
                SentAt = DateTime.UtcNow
            };

            // Find or create chat between sender and receiver
            var chat = await FindOrCreateDirectChatAsync(senderId, message.ReceiverId);
            chatMessage.ChatId = chat.Id;

            var savedMessage = await _messageRepository.AddAsync(chatMessage);

            // Map to DTO
            return new ChatMessageDto
            {
                Id = savedMessage.Id,
                SenderId = senderId,
                SenderName = sender.Name,
                ReceiverId = receiver.Id,
                ReceiverName = receiver.Name,
                Content = savedMessage.Content,
                SentAt = savedMessage.SentAt,
                IsRead = false
            };
        }

        //public async Task<IEnumerable<ChatMessageDto>> GetConversationAsync(Guid userId1, Guid userId2)
        //{
        //    // Find chat between users
        //    var chat = await _dbContext.Chats
        //        .Include(c => c.Participants)
        //        .Include(c => c.Messages)
        //        .ThenInclude(m => m.Sender)
        //        .Where(c => c.Participants.Count == 2 &&
        //                   c.Participants.Any(p => p.UserId == userId1) &&
        //                   c.Participants.Any(p => p.UserId == userId2))
        //        .FirstOrDefaultAsync();

        //    if (chat == null)
        //    {
        //        return Enumerable.Empty<ChatMessageDto>();
        //    }

        //    // Get user names
        //    var userIds = new List<Guid> { userId1, userId2 };
        //    var users = await _userRepository.GetByIdsAsync(userIds);
        //    var userNames = users.ToDictionary(u => u.Id, u => u.Name);

        //    // Map messages to DTOs
        //    return chat.Messages.Select(m => new ChatMessageDto
        //    {
        //        Id = m.Id,
        //        SenderId = m.SenderId,
        //        SenderName = userNames.GetValueOrDefault(m.SenderId, "Unknown User"),
        //        ReceiverId = (m.SenderId == userId1 ? userId2 : userId1),
        //        ReceiverName = userNames.GetValueOrDefault(m.SenderId == userId1 ? userId2 : userId1, "Unknown User"),
        //        Message = m.Content,
        //        SentAt = m.SentAt,
        //        IsRead = m.SentAt < DateTime.UtcNow.AddMinutes(-1) // Simple read status logic
        //    });
        //}

        public async Task<IEnumerable<ChatMessageDto>> GetMessagesForChatAsync(Guid chatId, Guid currentUserId)
        {
            var chat = await _dbContext.Chats
     .Include(c => c.Participants)
     .Include(c => c.Messages)
         .ThenInclude(m => m.Sender)
     .AsNoTracking()
     .FirstOrDefaultAsync(c => c.Id == chatId);



            if (chat == null)
            {
                return Enumerable.Empty<ChatMessageDto>(); // Unauthorized or not found
            }

            var userIds = chat.Participants.Select(p => p.UserId).Distinct().ToList();
            var users = await _userRepository.GetByIdsAsync(userIds);
            var userNames = users.ToDictionary(u => u.Id, u => u.Name);

            return chat.Messages.Select(m => new ChatMessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = userNames.GetValueOrDefault(m.SenderId, "Unknown User"),
                ReceiverId = chat.Participants.FirstOrDefault(p => p.UserId != m.SenderId)?.UserId ?? Guid.Empty,
                ReceiverName = userNames.GetValueOrDefault(
                    chat.Participants.FirstOrDefault(p => p.UserId != m.SenderId)?.UserId ?? Guid.Empty,
                    "Unknown User"
                ),
                Content = m.Content,
                SentAt = m.SentAt,
                IsRead = m.SentAt < DateTime.UtcNow.AddMinutes(-1),
                ChatId = chat.Id
            });
        }


        public async Task<IEnumerable<ChatConversationDto>> GetUserConversationsAsync(Guid userId)
        {
            // Get all chats where user is a participant
            var chats = await _dbContext.Chats
                .Include(c => c.Participants)
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .ToListAsync();

            var conversations = new List<ChatConversationDto>();

            foreach (var chat in chats)
            {
                // Get other participant
                var otherParticipant = chat.Participants.First(p => p.UserId != userId);
                var otherUser = await _userRepository.GetByIdAsync(otherParticipant.UserId);

                if (otherUser == null) continue;

                // Get last message
                var lastMessage = chat.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

                conversations.Add(new ChatConversationDto
                {
                    UserId = otherUser.Id,
                    UserName = otherUser.Name,
                    LastMessage = lastMessage?.Content ?? string.Empty,
                    LastMessageTime = lastMessage?.SentAt ?? DateTime.UtcNow,
                    UnreadCount = chat.Messages.Count(m => m.SenderId != userId && m.SentAt > DateTime.UtcNow.AddMinutes(-1))
                });
            }

            return conversations;
        }

        public async Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Messages)
                .Where(c => c.Participants.Count == 2 &&
                           c.Participants.Any(p => p.UserId == senderId) &&
                           c.Participants.Any(p => p.UserId == receiverId))
                .FirstOrDefaultAsync();

            if (chat != null)
            {
                var unreadMessages = chat.Messages
                    .Where(m => m.SenderId == senderId && m.SentAt > DateTime.UtcNow.AddMinutes(-1))
                    .ToList();

                foreach (var message in unreadMessages)
                {
                    message.SentAt = DateTime.UtcNow.AddMinutes(-2); // Mark as read
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            var chats = await _dbContext.Chats
                .Include(c => c.Messages)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .ToListAsync();

            return chats.Sum(chat => chat.Messages.Count(m => 
                m.SenderId != userId && 
                m.SentAt > DateTime.UtcNow.AddMinutes(-1)));
        }

        public async Task<IEnumerable<UserDto>> GetStaffMembersAsync()
        {
            var staffMembers = await _userRepository.GetUsersByRoleAsync("STAFF");
            return staffMembers.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            });
        }

        public async Task<IEnumerable<UserDto>> GetCustomersAsync()
        {
            var customers = await _userRepository.GetUsersByRoleAsync("CUSTOMER");
            return customers.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            });
        }

        public async Task<ChatMessageDto?> SendMessageToStaffAsync(Guid senderUserId, string messageContent)
        {
            var chat = await _chatRepository.FindChatByCustomerIdAsync(senderUserId);

            if (chat == null)
            {
                var created = await CreateStaffChatAsync(senderUserId);
                chat = await _chatRepository.GetByIdAsync(created.Id);
            }

            var message = new Message
            {
                ChatId = chat.Id,
                SenderId = senderUserId,
                Content = messageContent,
                SentAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);

            var sender = await _userRepository.GetByIdAsync(senderUserId);
            return new ChatMessageDto
            {
                Id = message.Id,
                ChatId = message.ChatId,
                SenderId = senderUserId,
                SenderName = sender?.Name ?? "Unknown",
                Content = messageContent,
                SentAt = message.SentAt,
                IsRead = false
            };
        }

        public async Task<ChatDto> CreateStaffChatAsync(Guid customerId)
        {
            var customer = await _userRepository.GetByIdAsync(customerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            // Kiểm tra xem đã có cuộc trò chuyện support nào chưa
            var existingChat = await _chatRepository.FindChatByCustomerIdAsync(customerId);
            if (existingChat != null)
            {
                return await GetChatByIdAsync(existingChat.Id);
            }

            // Tìm một nhân viên hỗ trợ khả dụng (chỉ lấy 1 người đầu tiên)
            var availableStaff = await _userRepository.FindAvailableStaffAsync();
            if (availableStaff == null)
                throw new InvalidOperationException("No available staff found");

            var chat = new Chat
            {
                Title = $"Support Chat - {customer.Name}",
                Status = ChatStatus.ACTIVE,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Participants = new List<ChatParticipant>
        {
            new ChatParticipant
            {
                UserId = customer.Id,
                JoinedAt = DateTime.UtcNow
            },
            new ChatParticipant
            {
                UserId = availableStaff.Id,
                JoinedAt = DateTime.UtcNow
            }
        }
            };

            await _chatRepository.AddAsync(chat);

            var fullChat = await _chatRepository.GetByIdAsync(chat.Id);
            return MapChatToDto(fullChat);
        }


        private async Task<Chat> FindOrCreateDirectChatAsync(Guid userId1, Guid userId2)
        {
            // Find existing chat
            var existingChat = await _dbContext.Chats
                .Include(c => c.Participants)
                .Where(c => c.Participants.Count == 2 &&
                           c.Participants.Any(p => p.UserId == userId1) &&
                           c.Participants.Any(p => p.UserId == userId2))
                .FirstOrDefaultAsync();

            if (existingChat != null)
            {
                return existingChat;
            }

            // Create new chat
            var chat = new Chat
            {
                Title = $"Direct Chat",
                Status = ChatStatus.ACTIVE,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add participants
            chat.Participants = new List<ChatParticipant>
            {
                new ChatParticipant { UserId = userId1, JoinedAt = DateTime.UtcNow },
                new ChatParticipant { UserId = userId2, JoinedAt = DateTime.UtcNow }
            };

            await _chatRepository.AddAsync(chat);
            return chat;
        }

        public async Task<IEnumerable<ChatMessageDto>> GetMessagesByChatIdAsync(Guid chatId)
        {

            var messages = await _messageRepository.GetMessagesForChatAsync(chatId);

            var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
            var senders = (await _userRepository.GetByIdsAsync(senderIds)).ToDictionary(u => u.Id, u => u.Name);

            return messages.Select(m => new ChatMessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                SenderId = m.SenderId,
                SenderName = senders.GetValueOrDefault(m.SenderId, "Unknown"),
                Content = m.Content,
                SentAt = m.SentAt,
                IsRead = false
            });
        }
        //

        public async Task<Guid> GetOrCreateChatWithUserAsync(Guid customerId)
        {
            // Kiểm tra xem đã có đoạn chat nào với customer này chưa
            var existingChat = await _dbContext.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c =>
                    c.Participants.Count == 2 &&
                    c.Participants.Any(p => p.UserId == customerId));

            if (existingChat != null)
                return existingChat.Id;

            // Tìm nhân viên hỗ trợ khả dụng
            var staff = await _userRepository.FindAvailableStaffAsync();
            if (staff == null)
                throw new Exception("No available staff found");

            // Tạo cuộc trò chuyện mới
            var chat = new Chat
            {
                Title = $"Support Chat - {customerId}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = ChatStatus.ACTIVE,
                Participants = new List<ChatParticipant>
        {
            new ChatParticipant { UserId = customerId, JoinedAt = DateTime.UtcNow },
            new ChatParticipant { UserId = staff.Id, JoinedAt = DateTime.UtcNow }
        }
            };

            await _chatRepository.AddAsync(chat);
            return chat.Id;
        }

    }
} 