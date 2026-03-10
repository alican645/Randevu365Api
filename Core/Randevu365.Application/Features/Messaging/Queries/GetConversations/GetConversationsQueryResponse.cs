namespace Randevu365.Application.Features.Messaging.Queries.GetConversations;

public class GetConversationsQueryResponseItem
{
    public string ConversationId { get; set; } = string.Empty;
    public int OtherUserId { get; set; }
    public int? AppointmentId { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageDate { get; set; }
}

public class GetConversationsQueryResponse
{
    public IList<GetConversationsQueryResponseItem> Items { get; set; } 
}
