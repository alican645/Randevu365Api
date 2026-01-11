using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessHours.Queries.GetBusinessHoursByBusinessId;

public class GetBusinessHoursByBusinessIdQueryRequest : IRequest<ApiResponse<List<GetBusinessHoursByBusinessIdQueryResponse>>>
{
    public int BusinessId { get; set; }
}
