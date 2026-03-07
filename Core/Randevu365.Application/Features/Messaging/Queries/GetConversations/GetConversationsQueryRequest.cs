using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Messaging.Queries.GetConversations;

public class GetConversationsQueryRequest : IRequest<ApiResponse<List<GetConversationsQueryResponse>>>
{
}
