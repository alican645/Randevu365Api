using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessLocations.Queries.GetBusinessLocationByBusinessId;

public class GetBusinessLocationByBusinessIdQueryRequest : IRequest<ApiResponse<IList<GetBusinessLocationByBusinessIdQueryResponse>>>
{
    public int BusinessId { get; set; }
}
