using Ecommerce.API.Services;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Claims;
using Ecommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.API.Apis
{
    public static class ChatApi
    {
        public static IEndpointRouteBuilder MapChatApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce/chat")
                         .HasApiVersion(1, 0)
                         .RequireAuthorization();

            // tất cả cuộc trò chuyện
            v1.MapGet("/conversations", async (IChatService chatService, HttpContext context) =>
            {
                var userId = GetUserIdFromClaims(context);
                if (userId == null) return Results.Unauthorized();

                var conversations = await chatService.GetUserConversationsAsync(userId.Value);
                return Results.Ok(conversations);
            });

            // lịch sử trò chuyện
            //v1.MapGet("/conversation/{chatId}", async (IChatService chatService, HttpContext context, Guid chatId) =>
            //{
            //    var currentUserId = GetUserIdFromClaims(context);
            //    if (currentUserId == null) return Results.Unauthorized();

            //    var messages = await chatService.GetMessagesForChatAsync(chatId, currentUserId.Value);
            //    return Results.Ok(messages);
            //});

            // đánh dấu là đã đọc
            v1.MapPost("/mark-read/{userId}", async (IChatService chatService, HttpContext context, Guid userId) =>
            {
                var currentUserId = GetUserIdFromClaims(context);
                if (currentUserId == null) return Results.Unauthorized();

                await chatService.MarkMessagesAsReadAsync(userId, currentUserId.Value);
                return Results.Ok();
            });

            // tin nhắn chưa đọc
            //v1.MapGet("/unread-count", async (IChatService chatService, HttpContext context) =>
            //{
            //    var userId = GetUserIdFromClaims(context);
            //    if (userId == null) return Results.Unauthorized();

            //    var count = await chatService.GetUnreadCountAsync(userId.Value);
            //    return Results.Ok(count);
            //});

            // gửi tin nhắn
            v1.MapPost("/send", async (IChatService chatService, HttpContext context, SendMessageDto message) =>
            {
                var senderId = GetUserIdFromClaims(context);
                if (senderId == null) return Results.Unauthorized();

                var savedMessage = await chatService.SendMessageAsync(senderId.Value, message);
                return Results.Ok(savedMessage);
            });



            // danh sách nhân viên hỗ trợ
            //v1.MapGet("/staff", async (IChatService chatService, HttpContext context) =>
            //{
            //    var userId = GetUserIdFromClaims(context);
            //    if (userId == null) return Results.Unauthorized();

            //    var staffMembers = await chatService.GetStaffMembersAsync();
            //    return Results.Ok(staffMembers);
            //});

            // danh sách khách hàng
            //v1.MapGet("/customers", async (IChatService chatService, HttpContext context) =>
            //{
            //    var userId = GetUserIdFromClaims(context);
            //    if (userId == null) return Results.Unauthorized();

            //    var customers = await chatService.GetCustomersAsync();
            //    return Results.Ok(customers);
            //});

            // tạo cuộc trò chuyện mới
            v1.MapPost("/create-staff-chat", async (IChatService chatService, HttpContext context) =>
            {
                var userId = GetUserIdFromClaims(context);
                if (userId == null) return Results.Unauthorized();

                var chat = await chatService.CreateStaffChatAsync(userId.Value);
                return Results.Ok(chat);
            });

            //lấy cuộc trò chuyện với admin
            v1.MapGet("/messages/{chatId}", async (IChatService chatService, HttpContext context, Guid chatId) =>
            {

                var messages = await chatService.GetMessagesByChatIdAsync(chatId);
                return Results.Ok(messages);
            });
            // lấy chat id dựa vào id user
            v1.MapGet("/chat-id-with/{otherUserId}", async (IChatService chatService, HttpContext context, Guid otherUserId) =>
            {
                var currentUserId = GetUserIdFromClaims(context);
                if (currentUserId == null) return Results.Unauthorized();

                var chatId = await chatService.GetOrCreateChatWithUserAsync(otherUserId);
                return Results.Ok(new { chatId });
            });



            return builder;
        }

        private static Guid? GetUserIdFromClaims(HttpContext context)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return null;
            return userId;
        }
    }

  
}
