using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessSummariesByOwner;

public class GetBusinessSummariesByOwnerQueryRequest : IRequest<ApiResponse<GetBusinessSummariesByOwnerQueryResponse>>
{
}
