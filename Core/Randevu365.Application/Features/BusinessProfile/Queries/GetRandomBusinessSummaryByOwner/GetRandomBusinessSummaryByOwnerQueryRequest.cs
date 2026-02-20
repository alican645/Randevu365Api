using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetRandomBusinessSummaryByOwner;

public class GetRandomBusinessSummaryByOwnerQueryRequest : IRequest<ApiResponse<GetRandomBusinessSummaryByOwnerQueryResponse>>
{
}
