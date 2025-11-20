using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Ganss.Xss;
using EzChat.Web.Services;

namespace EzChat.Web.Hubs;

/// <summary>
/// 실시간 채팅 허브
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly HtmlSanitizer _sanitizer;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
        _sanitizer = new HtmlSanitizer();
    }

    /// <summary>
    /// 채팅방에 입장
    /// </summary>
    public async Task JoinRoom(int roomId)
    {
        var room = await _chatService.GetRoomByIdAsync(roomId);
        if (room == null)
        {
            await Clients.Caller.SendAsync("Error", "채팅방을 찾을 수 없습니다.");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");

        var userName = GetUserDisplayName();
        await Clients.Group($"room_{roomId}").SendAsync("UserJoined", userName);

        _logger.LogInformation("User {UserId} joined room {RoomId}", GetUserId(), roomId);
    }

    /// <summary>
    /// 채팅방에서 퇴장
    /// </summary>
    public async Task LeaveRoom(int roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room_{roomId}");

        var userName = GetUserDisplayName();
        await Clients.Group($"room_{roomId}").SendAsync("UserLeft", userName);

        _logger.LogInformation("User {UserId} left room {RoomId}", GetUserId(), roomId);
    }

    /// <summary>
    /// 메시지 전송
    /// </summary>
    public async Task SendMessage(int roomId, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        // 메시지 길이 제한
        if (message.Length > 2000)
        {
            await Clients.Caller.SendAsync("Error", "메시지는 2000자를 초과할 수 없습니다.");
            return;
        }

        var userId = GetUserId();
        var sanitizedMessage = _sanitizer.Sanitize(message);

        // 메시지 저장
        var savedMessage = await _chatService.SaveMessageAsync(roomId, userId, sanitizedMessage);

        // 방에 브로드캐스트
        await Clients.Group($"room_{roomId}").SendAsync("ReceiveMessage", new
        {
            id = savedMessage.Id,
            userId = userId,
            userName = savedMessage.User?.DisplayName ?? savedMessage.User?.Email ?? "Unknown",
            content = savedMessage.Content,
            sentAt = savedMessage.SentAt.ToString("yyyy-MM-dd HH:mm:ss")
        });

        _logger.LogInformation("User {UserId} sent message in room {RoomId}", userId, roomId);
    }

    /// <summary>
    /// 연결 해제 시
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("User {UserId} disconnected", GetUserId());
        await base.OnDisconnectedAsync(exception);
    }

    private string GetUserId()
    {
        return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    private string GetUserDisplayName()
    {
        return Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
    }
}
