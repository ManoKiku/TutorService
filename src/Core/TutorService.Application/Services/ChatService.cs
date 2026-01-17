using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.Chat;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        IChatRepository chatRepository,
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        ITutorProfileRepository tutorProfileRepository,
        IMapper mapper,
        ILogger<ChatService> logger)
    {
        _chatRepository = chatRepository;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _tutorProfileRepository = tutorProfileRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ChatDto> CreateChatAsync(Guid userId, ChatCreateRequest request)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        
        var isUserTutor = tutorProfile is not null;
        
        if ((isUserTutor ? tutorProfile.Id == request.TutorId : false) && userId != request.StudentId)
            throw new UnauthorizedAccessException("You can only create chats involving yourself");

        var tutor = await _tutorProfileRepository.GetByIdAsync(request.TutorId);
        var student = await _userRepository.GetByIdAsync(request.StudentId);

        if (tutor == null)
            throw new KeyNotFoundException("Tutor not found");
        if (student == null)
            throw new KeyNotFoundException("Student not found");

        var existingChat = await _chatRepository.GetByParticipantsAsync(request.TutorId, request.StudentId);
        if (existingChat != null)
        {
            return _mapper.Map<ChatDto>(existingChat);
        }

        var chat = new Chat
        {
            TutorId = request.TutorId,
            StudentId = request.StudentId
        };

        await _chatRepository.AddAsync(chat);
        await _chatRepository.SaveChangesAsync();
        var chatWithDetails = await _chatRepository.GetByIdWithMessagesAsync(chat.Id);
        
        return _mapper.Map<ChatDto>(chatWithDetails!);
    }

    public async Task<ChatDto> GetByTutorId(Guid userId, Guid tutorId)
    {
        var chat = await  _chatRepository.GetByTutorId(userId, tutorId);

        if (chat == null)
            throw new KeyNotFoundException("No chat was found");

        return _mapper.Map<ChatDto>(chat);
    }

    public async Task<IEnumerable<ChatDto>> GetUserChatsAsync(Guid userId)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        
        if(tutorProfile != null)
            userId = tutorProfile.Id;
        
        var chats = await _chatRepository.GetUserChatsAsync(userId);

        var chatDtos = new List<ChatDto>();
        foreach (var chat in chats)
        {
            var dto = _mapper.Map<ChatDto>(chat);
            
            var lastMessage = chat.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
            if (lastMessage != null)
            {
                dto.LastMessage = _mapper.Map<MessageDto>(lastMessage);
            }
            
            dto.UnreadCount = await _chatRepository.GetUnreadCountAsync(chat.Id, userId);
            
            chatDtos.Add(dto);
        }

        return chatDtos;
    }

    public async Task<ChatDto?> GetChatByIdAsync(Guid chatId, Guid userId)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        
        if(tutorProfile != null)
            userId = tutorProfile.Id;
        
        if (!await _chatRepository.IsUserParticipantAsync(chatId, userId))
            throw new UnauthorizedAccessException("You are not a participant of this chat");

        var chat = await _chatRepository.GetByIdWithMessagesAsync(chatId);
        if (chat == null)
            return null;

        var dto = _mapper.Map<ChatDto>(chat);
        dto.UnreadCount = await _chatRepository.GetUnreadCountAsync(chatId, userId);
        
        return dto;
    }

    public async Task<IEnumerable<MessageDto>> GetChatMessagesAsync(Guid chatId, Guid userId, Guid? beforeMessageId = null)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        
        if(tutorProfile != null)
            userId = tutorProfile.Id;
        
        if (!await _chatRepository.IsUserParticipantAsync(chatId, userId))
            throw new UnauthorizedAccessException("You are not a participant of this chat");

        var messages = await _messageRepository.GetChatMessagesAsync(chatId, beforeMessageId);

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<int> GetUnreadCountAsync(Guid chatId, Guid userId)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        
        if(tutorProfile != null)
            userId = tutorProfile.Id;
        
        if (!await _chatRepository.IsUserParticipantAsync(chatId, userId))
            throw new UnauthorizedAccessException("You are not a participant of this chat");

        return await _chatRepository.GetUnreadCountAsync(chatId, userId);
    }

    public async Task MarkMessagesAsReadAsync(Guid chatId, Guid userId)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        
        if(tutorProfile != null)
            userId = tutorProfile.Id;
        
        if (!await _chatRepository.IsUserParticipantAsync(chatId, userId))
            throw new UnauthorizedAccessException("You are not a participant of this chat");

        await _chatRepository.MarkMessagesAsReadAsync(chatId, userId);
    }
}