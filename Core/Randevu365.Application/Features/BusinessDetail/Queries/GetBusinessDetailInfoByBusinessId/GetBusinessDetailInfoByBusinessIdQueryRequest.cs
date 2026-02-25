using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessDetailInfoByBusinessId;

public class GetBusinessDetailInfoByBusinessIdQueryRequest : IRequest<ApiResponse<GetBusinessDetailInfoByBusinessIdQueryResponse>>
{
    public int BusinessId { get; set; }
}
