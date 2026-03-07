using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Messaging.Queries.GetMessagesByConversation;

public class GetMessagesByConversationQueryRequest : IRequest<ApiResponse<List<GetMessagesByConversationQueryResponse>>>
{
    public required string ConversationId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
