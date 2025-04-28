using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Apis
{
    public static class ChatApi
    {
        public static IEndpointRouteBuilder MapChatApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce")
                         .HasApiVersion(1, 0)
                         .RequireAuthorization(); // Bắt buộc xác thực toàn bộ group này

            // Tạo Chat
            v1.MapPost("/chats", async ([FromBody] CreateChatRequest request,
                              IChatService chatService,
                              HttpContext httpContext) =>
            {
                var creatorId = GetUserIdFromClaims(httpContext);
                if (creatorId == null)
                    return Results.Unauthorized();

                try
                {
                    var chatDto = await chatService.CreateChatAsync(request, creatorId.Value);
                    return Results.Created($"/api/chats/{chatDto.Id}", chatDto);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception)
                {
                    return Results.Problem("An error occurred while creating the chat.");
                }
            });


            // Lấy danh sách chats của User
            v1.MapGet("/user/{userId:guid}", async (
                Guid userId,
                IChatService chatService,
                HttpContext httpContext) =>
            {
                var callerId = GetUserIdFromClaims(httpContext);
                if (callerId == null) return Results.Unauthorized();
                if (callerId != userId) return Results.Forbid(); // Không cho phép xem chat người khác

                var chats = await chatService.GetChatsForUserAsync(userId);
                return Results.Ok(chats);
            })
            .WithName("GetChatsForUser")
            .Produces<IEnumerable<ChatDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

            // Lấy thông tin chi tiết Chat
            v1.MapGet("/{chatId:guid}", async (
                Guid chatId,
                IChatService chatService,
                HttpContext httpContext) =>
            {
                var userId = GetUserIdFromClaims(httpContext);
                if (userId == null) return Results.Unauthorized();

                var chat = await chatService.GetChatByIdAsync(chatId);
                if (chat == null) return Results.NotFound();

                if (!chat.Participants.Any(p => p.UserId == userId))
                {
                    return Results.Forbid();
                }

                return Results.Ok(chat);
            })
            .WithName("GetChatById")
            .Produces<ChatDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

            // Lấy danh sách Messages trong Chat
            v1.MapGet("/{chatId:guid}/messages", async (
                Guid chatId,
                [FromQuery] int skip,
                [FromQuery] int take,
                IChatService chatService,
                HttpContext httpContext) =>
            {
                var userId = GetUserIdFromClaims(httpContext);
                if (userId == null) return Results.Unauthorized();

                var chat = await chatService.GetChatByIdAsync(chatId);
                if (chat == null) return Results.NotFound();

                if (!chat.Participants.Any(p => p.UserId == userId))
                {
                    return Results.Forbid();
                }

                var messages = await chatService.GetMessagesForChatAsync(chatId, userId.Value, skip, take);
                return Results.Ok(messages);
            })
            .WithName("GetChatMessages")
            .Produces<IEnumerable<MessageDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

            // Tham gia Chat
            v1.MapPost("/{chatId:guid}/join", async (
                Guid chatId,
                IChatService chatService,
                HttpContext httpContext) =>
            {
                var userId = GetUserIdFromClaims(httpContext);
                if (userId == null) return Results.Unauthorized();

                var request = new JoinChatRequest { ChatId = chatId, UserId = userId.Value };
                var success = await chatService.JoinChatAsync(request);

                return success
                    ? Results.Ok("Successfully joined the chat.")
                    : Results.BadRequest("Failed to join chat (chat not found or already a participant).");
            })
            .WithName("JoinChat")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

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
    public class CreateChatMessageDto
    {
        public Guid ChatId { get; set; }
        public string Content { get; set; }
        public Guid SenderId { get; set; }
    }
}
