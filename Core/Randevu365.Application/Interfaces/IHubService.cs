
namespace Randevu365.Application.Interfaces;

public interface IHubService
{
    Task SaveMessage(
        int senderId,
        int receiverId,
        string message,
        string conversationId);
}