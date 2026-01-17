using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs.Chat;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IMessageService _messageService;
    private readonly ILogger<ChatsController> _logger;

    public ChatsController(
        IChatService chatService,
        IMessageService messageService,
        ILogger<ChatsController> logger)
    {
        _chatService = chatService;
        _messageService = messageService;
        _logger = logger;
    }
    
    [HttpGet("tutor/{tutorId}")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ChatDto>> GetByTutorId(Guid tutorId)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        var chat = await _chatService.GetByTutorId(userId, tutorId);
        return Ok(chat);
    }

    [HttpPost]
    [Authorize(Roles = "Student,Tutor")]
    public async Task<ActionResult<ChatDto>> CreateChat(ChatCreateRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        var chat = await _chatService.CreateChatAsync(userId, request);
        return CreatedAtAction(nameof(GetChat), new { id = chat.Id }, chat);
    }

    [HttpGet("my-chats")]
    public async Task<ActionResult<IEnumerable<ChatDto>>> GetMyChats()
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);;
        var response = await _chatService.GetUserChatsAsync(userId);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChatDto>> GetChat(Guid id)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);;
        var chat = await _chatService.GetChatByIdAsync(id, userId);
        
        if (chat == null)
            return NotFound();
            
        return Ok(chat);
    }

    [HttpPost("{chatId}/messages")]
    public async Task<ActionResult<MessageDto>> SendMessage(Guid chatId, MessageCreateRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);;
        var message = await _messageService.SendMessageAsync(chatId, userId, request);
        return CreatedAtAction(null, new { chatId, messageId = message.Id }, message);
    }

    [HttpGet("{chatId}/messages")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetChatMessages(
        Guid chatId,
        [FromQuery] Guid? beforeMessageId = null)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);;
        var response = await _chatService.GetChatMessagesAsync(chatId, userId, beforeMessageId);
        return Ok(response);
}

    [HttpPut("{chatId}/messages/mark-read")]
    public async Task<IActionResult> MarkMessagesAsRead(Guid chatId)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);;
        await _chatService.MarkMessagesAsReadAsync(chatId, userId);
        return NoContent();
    }

    [HttpGet("{chatId}/unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount(Guid chatId)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);;
        var count = await _chatService.GetUnreadCountAsync(chatId, userId);
        return Ok(count);
    }

    [HttpDelete("{chatId}/messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(Guid chatId, Guid messageId)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);;
        var result = await _messageService.DeleteMessageAsync(messageId, userId);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }
}
