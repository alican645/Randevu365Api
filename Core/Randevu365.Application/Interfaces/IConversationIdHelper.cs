
namespace Randevu365.Application.Interfaces;

public interface IConversationIdHelper
{
    Task<string> GenerateConversationId(int senderId, int receiverId);
}
