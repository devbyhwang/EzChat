using Microsoft.EntityFrameworkCore;
using Ganss.Xss;
using EzChat.Web.Data;
using EzChat.Web.Models;
using EzChat.Web.ViewModels;

namespace EzChat.Web.Services;

/// <summary>
/// 채팅 서비스 구현
/// </summary>
public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly HtmlSanitizer _sanitizer;
    private readonly ILogger<ChatService> _logger;

    public ChatService(ApplicationDbContext context, ILogger<ChatService> logger)
    {
        _context = context;
        _logger = logger;
        _sanitizer = new HtmlSanitizer();
    }

    public async Task<IEnumerable<ChatRoom>> GetActiveRoomsAsync()
    {
        return await _context.ChatRooms
            .Include(r => r.CreatedBy)
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<ChatRoom?> GetRoomByIdAsync(int id)
    {
        return await _context.ChatRooms
            .Include(r => r.CreatedBy)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
    }

    public async Task<ChatRoom> CreateRoomAsync(CreateRoomViewModel model, string userId)
    {
        var room = new ChatRoom
        {
            Name = _sanitizer.Sanitize(model.Name),
            Description = model.Description != null ? _sanitizer.Sanitize(model.Description) : null,
            CreatedById = userId,
            MaxUsers = model.MaxUsers
        };

        _context.ChatRooms.Add(room);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Room {RoomId} created by user {UserId}", room.Id, userId);
        return room;
    }

    public async Task<bool> DeleteRoomAsync(int id, string userId, bool isAdmin = false)
    {
        var room = await _context.ChatRooms.FindAsync(id);
        if (room == null || !room.IsActive) return false;

        // 생성자 또는 관리자만 삭제 가능
        if (room.CreatedById != userId && !isAdmin) return false;

        room.IsActive = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Room {RoomId} deleted by user {UserId}", id, userId);
        return true;
    }

    public async Task<ChatMessage> SaveMessageAsync(int roomId, string userId, string content)
    {
        var message = new ChatMessage
        {
            RoomId = roomId,
            UserId = userId,
            Content = _sanitizer.Sanitize(content)
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        // User 정보를 포함하여 반환
        await _context.Entry(message).Reference(m => m.User).LoadAsync();

        return message;
    }

    public async Task<IEnumerable<ChatMessage>> GetRecentMessagesAsync(int roomId, int count = 50)
    {
        return await _context.ChatMessages
            .Include(m => m.User)
            .Where(m => m.RoomId == roomId && !m.IsDeleted)
            .OrderByDescending(m => m.SentAt)
            .Take(count)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}
