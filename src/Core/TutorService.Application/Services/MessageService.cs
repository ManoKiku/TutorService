using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs;
using TutorService.Application.DTOs.Chat;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IChatRepository _chatRepository;
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<MessageService> _logger;

    public MessageService(
        IMessageRepository messageRepository,
        IChatRepository chatRepository,
        ITutorProfileRepository tutorProfileRepository,
        IMapper mapper,
        ILogger<MessageService> logger)
    {
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
        _mapper = mapper;
        _logger = logger;
        _tutorProfileRepository = tutorProfileRepository;
    }

    public async Task<MessageDto> SendMessageAsync(Guid chatId, Guid senderId, MessageCreateRequest request)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(senderId);
        
        if (!await _chatRepository.IsUserParticipantAsync(chatId, tutorProfile != null ? tutorProfile.Id : senderId))
            throw new UnauthorizedAccessException("You are not a participant of this chat");

        var message = new Message
        {
            ChatId = chatId,
            SenderId = senderId,
            Text = request.Text,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        await _messageRepository.AddAsync(message);
        await _messageRepository.SaveChangesAsync();
        
        var messageWithDetails = await _messageRepository.GetByIdWithDetailsAsync(message.Id);
        
        return _mapper.Map<MessageDto>(messageWithDetails!);
    }

    public async Task<bool> DeleteMessageAsync(Guid messageId, Guid userId)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        
        if(tutorProfile != null)
            userId = tutorProfile.Id;
        
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null)
            return false;

        if (!await _messageRepository.IsMessageSenderAsync(messageId, userId))
            throw new UnauthorizedAccessException("You can only delete your own messages");

        _messageRepository.Remove(message);
        await _messageRepository.SaveChangesAsync();
        return true;
    }

    public async Task<MessageDto?> GetMessageByIdAsync(Guid messageId, Guid userId)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        
        if(tutorProfile != null)
            userId = tutorProfile.Id;
        
        var message = await _messageRepository.GetByIdWithDetailsAsync(messageId);
        if (message == null)
            return null;
        
        if (!await _chatRepository.IsUserParticipantAsync(message.ChatId, userId))
            throw new UnauthorizedAccessException("You are not a participant of this chat");

        return _mapper.Map<MessageDto>(message);
    }
}