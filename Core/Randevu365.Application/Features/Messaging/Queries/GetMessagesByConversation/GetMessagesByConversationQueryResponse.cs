namespace Randevu365.Application.Features.Messaging.Queries.GetMessagesByConversation;

public class GetMessagesByConversationQueryResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public DateTime SentAt { get; set; }
}
