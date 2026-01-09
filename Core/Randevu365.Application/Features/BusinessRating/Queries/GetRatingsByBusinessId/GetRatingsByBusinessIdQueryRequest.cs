using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessRating.Queries.GetRatingsByBusinessId;

public class GetRatingsByBusinessIdQueryRequest : IRequest<ApiResponse<IList<GetRatingsByBusinessIdQueryResponse>>>
{
    public int BusinessId { get; set; }
}
