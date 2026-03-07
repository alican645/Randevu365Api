namespace Randevu365.Application.Features.Messaging.Queries.GetConversations;

public class GetConversationsQueryResponse
{
    public string ConversationId { get; set; } = string.Empty;
    public int OtherUserId { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageDate { get; set; }
}
