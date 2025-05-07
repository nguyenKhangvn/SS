using Ecommerce.Infrastructure.Models.Dtos;
using Ecommerce.Infrastructure.Models;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatParticipantRepository _participantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMapper _mapper;

        public ChatService(
            IChatRepository chatRepository,
            IMessageRepository messageRepository,
            IChatParticipantRepository participantRepository,
            IUserRepository userRepository,
            IHubContext<ChatHub> hubContext,
            IMapper mapper)
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _participantRepository = participantRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task<ChatDto?> GetChatByIdAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null)
            {
                return null;
            }
            return _mapper.Map<ChatDto>(chat);
        }

        public async Task<IEnumerable<ChatDto>> GetChatsForUserAsync(Guid userId)
        {
            var chats = await _chatRepository.GetChatsForUserAsync(userId);
            return _mapper.Map<IEnumerable<ChatDto>>(chats);
        }

        //public async Task<ChatDto> CreateChatAsync(CreateChatRequest request, Guid creatorUserId)
        //{
        //    var participantUserIds = request.ParticipantUserIds.Concat(new[] { creatorUserId }).Distinct().ToList();
        //    var participantUsers = await _userRepository.GetByIdsAsync(participantUserIds);

        //    if (participantUsers.Count() != participantUserIds.Count)
        //    {
        //        throw new ArgumentException("One or more participant user IDs are invalid.");
        //    }

        //    var chat = new Chat
        //    {
        //        Title = request.Title,
        //        Status = ChatStatus.ACTIVE,
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow
        //    };

        //    await _chatRepository.AddAsync(chat);

        //    foreach (var userId in participantUserIds)
        //    {
        //        var participant = new ChatParticipant
        //        {
        //            ChatId = chat.Id,
        //            UserId = userId,
        //            JoinedAt = DateTime.UtcNow
        //        };
        //        await _participantRepository.AddAsync(participant);
        //    }

        //    var createdChat = await _chatRepository.GetByIdAsync(chat.Id);
        //    return _mapper.Map<ChatDto>(createdChat);
        //}

        //public async Task<ChatMessageDto?> SendMessageAsync(Guid senderUserId, SendMessageRequest request)
        //{
        //    Guid chatId;
        //    Chat? chat;

        //    if (request.ChatId == Guid.Empty)
        //    {
        //        chat = await _chatRepository.FindChatByCustomerIdAsync(senderUserId);
        //        if (chat == null)
        //        {
        //            var availableStaff = await _userRepository.FindAvailableStaffAsync();
        //            if (availableStaff == null)
        //                return null;

        //            chat = new Chat
        //            {
        //                Id = Guid.NewGuid(),
        //                Title = "Hỗ trợ",
        //                Status = ChatStatus.ACTIVE,
        //                CreatedAt = DateTime.UtcNow,
        //                UpdatedAt = DateTime.UtcNow
        //            };

        //            await _chatRepository.AddAsync(chat);
        //            await _participantRepository.AddParticipantAsync(chat.Id, senderUserId);
        //            await _participantRepository.AddParticipantAsync(chat.Id, availableStaff.Id);
        //        }
        //        chatId = chat.Id;
        //    }
        //    else
        //    {
        //        chatId = request.ChatId;
        //        chat = await _chatRepository.GetByIdAsync(chatId);
        //        if (chat == null)
        //            return null;
        //    }

        //    var message = new Message
        //    {
        //        ChatId = chatId,
        //        SenderId = senderUserId,
        //        Content = request.Content,
        //        SentAt = DateTime.UtcNow
        //    };

        //    var savedMessage = await _messageRepository.AddAsync(message);

        //    var sender = await _userRepository.GetByIdAsync(senderUserId);
        //    if (sender == null)
        //        return null;

        //    var participants = await _participantRepository.GetParticipantsForChatAsync(chatId);
        //    var receiverParticipant = participants.FirstOrDefault(p => p.UserId != senderUserId);
        //    User? receiverUser = null;
        //    if (receiverParticipant != null)
        //    {
        //        receiverUser = await _userRepository.GetByIdAsync(receiverParticipant.UserId);
        //    }

        //    return new ChatMessageDto
        //    {
        //        Id = savedMessage.Id,
        //        ChatId = savedMessage.ChatId,
        //        SenderId = savedMessage.SenderId,
        //        SenderName = sender.Name,
        //        ReceiverId = receiverUser?.Id ?? Guid.Empty,
        //        ReceiverName = receiverUser?.Name ?? null,
        //        Content = savedMessage.Content,
        //        SentAt = savedMessage.SentAt,
        //        IsRead = false
        //    };
        //}

        public async Task<bool> JoinChatAsync(JoinChatRequest request)
        {
            var chatExists = await _chatRepository.ExistsAsync(request.ChatId);
            if (!chatExists)
            {
                return false;
            }

            var isParticipant = await _participantRepository.IsUserInChatAsync(request.ChatId, request.UserId);
            if (isParticipant)
            {
                return false;
            }

            await _participantRepository.AddParticipantAsync(request.ChatId, request.UserId);
            return true;
        }

        //public async Task<IEnumerable<MessageDto>> GetMessagesForChatAsync(Guid chatId, Guid userId, int skip = 0, int take = 50)
        //{
        //    var isParticipant = await _participantRepository.IsUserInChatAsync(chatId, userId);
        //    if (!isParticipant)
        //    {
        //        return Enumerable.Empty<MessageDto>();
        //    }

        //    var messages = await _messageRepository.GetMessagesForChatAsync(chatId, skip, take);

        //    var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
        //    var senders = (await _userRepository.GetByIdsAsync(senderIds)).ToDictionary(u => u.Id, u => u.Name);

        //    var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages).ToList();

        //    foreach (var messageDto in messageDtos)
        //    {
        //        messageDto.SenderName = senders.GetValueOrDefault(messageDto.SenderId, "Unknown User");
        //    }

        //    return messageDtos;
        //}

        //public async Task<ChatDto> FindOrCreateAdminChatAsync(Guid userId, Guid adminUserId)
        //{
        //    var currentUser = await _userRepository.GetByIdAsync(userId);
        //    if (currentUser == null)
        //    {
        //        throw new ArgumentException($"User with ID {userId} not found.");
        //    }

        //    var adminUser = await _userRepository.GetByIdAsync(adminUserId);
        //    if (adminUser == null)
        //    {
        //        throw new ArgumentException($"Admin user with ID {adminUserId} not found.");
        //    }

        //    var chat = await _chatRepository.FindOrCreateDirectChatAsync(userId, adminUserId);

        //    if (string.IsNullOrEmpty(chat.Title) || chat.Title == "Direct Chat")
        //    {
        //        chat.Title = "Support Chat";
        //        await _chatRepository.UpdateAsync(chat);
        //    }

        //    var updatedChat = await _chatRepository.GetByIdAsync(chat.Id);
        //    return _mapper.Map<ChatDto>(updatedChat);
        //}

       


        //public async Task<IEnumerable<ChatMessageDto>> GetMessagesForChatAsync(Guid chatId, Guid currentUserId)
        //{
        //    var isParticipant = await _participantRepository.IsUserInChatAsync(chatId, currentUserId);
        //    if (!isParticipant)
        //    {
        //        return Enumerable.Empty<ChatMessageDto>();
        //    }

        //    var chat = await _chatRepository.GetByIdAsync(chatId);
        //    if (chat == null)
        //    {
        //        return Enumerable.Empty<ChatMessageDto>();
        //    }

        //    var participants = await _participantRepository.GetParticipantsForChatAsync(chatId);
        //    var messages = await _messageRepository.GetMessagesForChatAsync(chatId);

        //    var userIds = participants.Select(p => p.UserId).Distinct().ToList();
        //    var users = await _userRepository.GetByIdsAsync(userIds);
        //    var userNames = users.ToDictionary(u => u.Id, u => u.Name);

        //    var chatMessageDtos = new List<ChatMessageDto>();

        //    foreach (var message in messages)
        //    {
        //        var dto = _mapper.Map<ChatMessageDto>(message);

        //        dto.SenderName = userNames.GetValueOrDefault(message.SenderId, "Unknown User");

        //        var receiverParticipant = participants.FirstOrDefault(p => p.UserId != message.SenderId);
        //        if (receiverParticipant != null)
        //        {
        //            dto.ReceiverId = receiverParticipant.UserId;
        //            dto.ReceiverName = userNames.GetValueOrDefault(receiverParticipant.UserId, "Unknown User");
        //        }
        //        else
        //        {
        //            dto.ReceiverId = Guid.Empty;
        //            dto.ReceiverName = null;
        //        }

        //        dto.IsRead = message.SentAt < DateTime.UtcNow.AddMinutes(-1);

        //        chatMessageDtos.Add(dto);
        //    }

        //    return chatMessageDtos;
        //}

        public async Task<IEnumerable<ChatConversationDto>> GetUserConversationsAsync(Guid userId)
        {
            return await _chatRepository.GetUserConversationsAsync(userId);
        }

        public async Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId)
        {
            var chat = await _chatRepository.FindChatByCustomerIdAsync(receiverId);
            if (chat != null)
            {
                await _messageRepository.MarkMessagesAsReadAsync(chat.Id , senderId);
            }
        }

        //public async Task<int> GetUnreadCountAsync(Guid userId)
        //{
        //    return await _chatRepository.GetUnreadCountAsync(userId);
        //}

        //public async Task<IEnumerable<UserDto>> GetStaffMembersAsync()
        //{
        //    var staffMembers = await _userRepository.GetUsersByRoleAsync("STAFF");
        //    return _mapper.Map<IEnumerable<UserDto>>(staffMembers);
        //}

        //public async Task<IEnumerable<UserDto>> GetCustomersAsync()
        //{
        //    var customers = await _userRepository.GetUsersByRoleAsync("CUSTOMER");
        //    return _mapper.Map<IEnumerable<UserDto>>(customers);
        //}

        //public async Task<ChatMessageDto?> SendMessageToStaffAsync(Guid senderUserId, string messageContent)
        //{
        //    var chat = await _chatRepository.FindChatByCustomerIdAsync(senderUserId);

        //    if (chat == null)
        //    {
        //        var createdChatDto = await CreateStaffChatAsync(senderUserId);
        //        chat = await _chatRepository.GetByIdAsync(createdChatDto.Id);
        //        if (chat == null) return null;
        //    }

        //    var message = new Message
        //    {
        //        ChatId = chat.Id,
        //        SenderId = senderUserId,
        //        Content = messageContent,
        //        SentAt = DateTime.UtcNow
        //    };

        //    var savedMessage = await _messageRepository.AddAsync(message);

        //    var sender = await _userRepository.GetByIdAsync(senderUserId);

        //    var participants = await _participantRepository.GetParticipantsForChatAsync(chat.Id);
        //    var receiverParticipant = participants.FirstOrDefault(p => p.UserId != senderUserId);
        //    User? receiverUser = null;
        //    if (receiverParticipant != null)
        //    {
        //        receiverUser = await _userRepository.GetByIdAsync(receiverParticipant.UserId);
        //    }

        //    return new ChatMessageDto
        //    {
        //        Id = savedMessage.Id,
        //        ChatId = savedMessage.ChatId,
        //        SenderId = senderUserId,
        //        SenderName = sender?.Name ?? "Unknown",
        //        ReceiverId = receiverUser?.Id ?? Guid.Empty,
        //        ReceiverName = receiverUser?.Name ?? null,
        //        Content = messageContent,
        //        SentAt = savedMessage.SentAt,
        //        IsRead = false
        //    };
        //}

        public async Task<ChatDto> CreateStaffChatAsync(Guid customerId)
        {
            var customer = await _userRepository.GetByIdAsync(customerId);
            if (customer == null)
                throw new ArgumentException("Khách hàng không hợp lệ");

            // Kiểm tra xem đã có cuộc trò chuyện nào chưa
            var existingChat = await _chatRepository.FindChatByCustomerIdAsync(customer.Id);
            if (existingChat != null)
            {
                return _mapper.Map<ChatDto>(existingChat);
            }

            // Tạo cuộc trò chuyện mới
            var chat = new Chat
            {
                Title = $"Hỗ trợ - {customer.Name}",
                Status = ChatStatus.ACTIVE,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _chatRepository.AddAsync(chat);

            // Chọn ngẫu nhiên một nhân viên hỗ trợ
            var availableStaff = await _userRepository.FindAvailableStaffAsync();
            if (availableStaff == null)
                throw new Exception("Không có nhân viên hỗ trợ nào có sẵn.");

            // Thêm người tham gia cuộc trò chuyện
            await _participantRepository.AddParticipantAsync(chat.Id, customer.Id);
            await _participantRepository.AddParticipantAsync(chat.Id, availableStaff.Id); 

            var fullChat = await _chatRepository.GetByIdAsync(chat.Id);
            return _mapper.Map<ChatDto>(fullChat);
        }


        public async Task<IEnumerable<ChatMessageDto>> GetMessagesByChatIdAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null) return Enumerable.Empty<ChatMessageDto>();

            var participants = await _participantRepository.GetParticipantsForChatAsync(chatId);
            var messages = await _messageRepository.GetMessagesForChatAsync(chatId);

            var userIds = participants.Select(p => p.UserId).Distinct().ToList();
            var users = await _userRepository.GetByIdsAsync(userIds);
            var userNames = users.ToDictionary(u => u.Id, u => u.Name);

            var chatMessageDtos = new List<ChatMessageDto>();

            foreach (var message in messages)
            {
                var dto = _mapper.Map<ChatMessageDto>(message);

                dto.SenderName = userNames.GetValueOrDefault(message.SenderId, "Unknown");

                var receiverParticipant = participants.FirstOrDefault(p => p.UserId != message.SenderId);
                if (receiverParticipant != null)
                {
                    dto.ReceiverId = receiverParticipant.UserId;
                    dto.ReceiverName = userNames.GetValueOrDefault(receiverParticipant.UserId, "Unknown User");
                }
                else
                {
                    dto.ReceiverId = Guid.Empty;
                    dto.ReceiverName = null;
                }

                dto.IsRead = message.SentAt < DateTime.UtcNow.AddMinutes(-1);

                chatMessageDtos.Add(dto);
            }

            return chatMessageDtos;
        }
        // lấy chat id dựa vào id user
        public async Task<Guid> GetOrCreateChatWithUserAsync(Guid customerId)
        {
            var existingChat = await _chatRepository.FindChatByCustomerIdAsync(customerId);
            if (existingChat != null)
                return existingChat.Id;

            var staff = await _userRepository.FindAvailableStaffAsync();
            if (staff == null)
                throw new Exception("No available staff found");

            var customer = await _userRepository.GetByIdAsync(customerId);
            if (customer == null)
                throw new ArgumentException($"Customer with ID {customerId} not found");

            var chat = new Chat
            {
                Title = $"Support Chat - {customer.Name}",
                Status = ChatStatus.ACTIVE,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _chatRepository.AddAsync(chat);

            await _participantRepository.AddParticipantAsync(chat.Id, customerId);
            await _participantRepository.AddParticipantAsync(chat.Id, staff.Id);

            return chat.Id;
        }

        public async Task<ChatMessageDto> SendMessageAsync(Guid senderId, SendMessageDto message)
        {
            var receiver = await _userRepository.GetByIdAsync(message.ReceiverId);
            if (receiver == null)
            {
                throw new ArgumentException($"id nguoi nhan - {message.ReceiverId} not found.");
            }

            var sender = await _userRepository.GetByIdAsync(senderId);
            if (sender == null)
            {
                throw new ArgumentException($"id nguoi gui {senderId} not found.");
            }

            var isSenderCustomer = await IsCustomer(senderId);
            var chat = isSenderCustomer
                ? await _chatRepository.FindChatByCustomerIdAsync(senderId)
                : await _chatRepository.FindChatByCustomerIdAsync(message.ReceiverId);


            var chatMessage = new Message
            {
                ChatId = chat.Id,
                SenderId = senderId,
                Content = message.Content,
                SentAt = DateTime.UtcNow
            };

            var savedMessage = await _messageRepository.AddAsync(chatMessage);

            var messageDto = new ChatMessageDto
            {
                Id = savedMessage.Id,
                ChatId = chat.Id,
                SenderId = senderId,
                SenderName = sender.Name,
                ReceiverId = receiver.Id,
                ReceiverName = receiver.Name,
                Content = savedMessage.Content,
                SentAt = savedMessage.SentAt,
                IsRead = false
            };

            await _hubContext.Clients.Group(chat.Id.ToString())
                .SendAsync("ReceiveMessage", messageDto);

            return messageDto;
        }

        public async Task<bool> IsCustomer(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            return user?.Role == RoleStatus.CUSTOMER;
        }
    }
}