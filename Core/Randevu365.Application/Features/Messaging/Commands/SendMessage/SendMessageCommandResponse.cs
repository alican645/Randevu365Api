namespace Randevu365.Application.Features.Messaging.Commands.SendMessage;

public class SendMessageCommandResponse
{
    public int MessageId { get; set; }
    public string ConversationId { get; set; } = string.Empty;
}
