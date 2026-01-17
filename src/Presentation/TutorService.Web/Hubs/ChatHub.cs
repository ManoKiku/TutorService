using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TutorService.Application.DTOs.Chat;
using TutorService.Application.Interfaces;
using TutorService.Domain.Interfaces;
using TutorService.Web.Helpers;

namespace TutorService.Web.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly IMessageService _messageService;
    private readonly ITutorProfileRepository  _tutorProfileRepository;
    private readonly ILogger<ChatHub> _logger;
    private static readonly Dictionary<Guid, string> _userConnections = new();

    public ChatHub(
        IChatService chatService,
        IMessageService messageService,
        ITutorProfileRepository tutorProfileRepository,
        ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _messageService = messageService;
        _tutorProfileRepository = tutorProfileRepository;
        _logger = logger;
    }
    

    public override async Task OnConnectedAsync()
    {
        var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
        _userConnections[userId] = Context.ConnectionId;
        
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
        _userConnections.Remove(userId);
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        _logger.LogInformation("User {UserId} disconnected", userId);
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        try
        {
            var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
            
            var messageDto = await _messageService.SendMessageAsync(
                request.ChatId, 
                userId, 
                new MessageCreateRequest { Text = request.Text });
            
            var chat = await _chatService.GetChatByIdAsync(request.ChatId, userId);
            if (chat == null)
                return;

            var tutorProfile = await _tutorProfileRepository.GetByIdAsync(chat.TutorId);
            
            if (tutorProfile == null)
                throw new KeyNotFoundException("Tutor not found");

            var otherUserId = tutorProfile.UserId == userId ? chat.StudentId : tutorProfile.UserId;
            
            await Clients.Caller.SendAsync("ReceiveMessage", new MessageReceivedDto
            {
                MessageId = messageDto.Id,
                ChatId = messageDto.ChatId,
                SenderId = messageDto.SenderId,
                SenderName = messageDto.SenderName,
                Text = messageDto.Text,
                SentAt = messageDto.SentAt
            });
            
            if (_userConnections.TryGetValue(otherUserId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", new MessageReceivedDto
                {
                    MessageId = messageDto.Id,
                    ChatId = messageDto.ChatId,
                    SenderId = messageDto.SenderId,
                    SenderName = messageDto.SenderName,
                    Text = messageDto.Text,
                    SentAt = messageDto.SentAt
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message in SignalR hub");
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    public async Task MarkMessagesAsRead(Guid chatId)
    {
        try
        {
            var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
            
            await _chatService.MarkMessagesAsReadAsync(chatId, userId);
            
            var chat = await _chatService.GetChatByIdAsync(chatId, userId);
            if (chat == null)
                return;

            var otherUserId = chat.TutorId == userId ? chat.StudentId : chat.TutorId;
            
            if (_userConnections.TryGetValue(otherUserId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("MessagesRead", new ReadReceiptDto
                {
                    ChatId = chatId,
                    UserId = userId,
                    ReadAt = DateTime.UtcNow
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking messages as read in SignalR hub");
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    public async Task<bool> IsUserOnline(Guid userId)
    {
        return _userConnections.ContainsKey(userId);
    }
}
