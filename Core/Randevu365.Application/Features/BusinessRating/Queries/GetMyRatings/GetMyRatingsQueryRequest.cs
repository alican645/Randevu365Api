using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessRating.Queries.GetMyRatings;

public class GetMyRatingsQueryRequest : IRequest<ApiResponse<IList<GetMyRatingsQueryResponse>>>
{
    // UserId JWT'den alınacak
}
