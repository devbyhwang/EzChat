using EzChat.Web.Models;
using EzChat.Web.ViewModels;

namespace EzChat.Web.Services;

/// <summary>
/// 채팅 서비스 인터페이스
/// </summary>
public interface IChatService
{
    Task<IEnumerable<ChatRoom>> GetActiveRoomsAsync();
    Task<ChatRoom?> GetRoomByIdAsync(int id);
    Task<ChatRoom> CreateRoomAsync(CreateRoomViewModel model, string userId);
    Task<bool> DeleteRoomAsync(int id, string userId, bool isAdmin = false);
    Task<ChatMessage> SaveMessageAsync(int roomId, string userId, string content);
    Task<IEnumerable<ChatMessage>> GetRecentMessagesAsync(int roomId, int count = 50);
}
