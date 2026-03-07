using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Messaging.Commands.SendMessage;

public class SendMessageCommandRequest : IRequest<ApiResponse<SendMessageCommandResponse>>
{
    public int ReceiverId { get; set; }
    public required string Content { get; set; }
}
