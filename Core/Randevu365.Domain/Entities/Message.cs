namespace Randevu365.Domain.Entities;

using Randevu365.Domain.Base;
public class Message : BaseEntity
{
    public string Content { get; set; }
    public string ConversationId { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
}
