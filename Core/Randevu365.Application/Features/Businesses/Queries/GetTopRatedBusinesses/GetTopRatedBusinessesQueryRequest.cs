using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Queries.GetTopRatedBusinesses;

public class GetTopRatedBusinessesQueryRequest : IRequest<ApiResponse<GetTopRatedBusinessesQueryResponse>>
{
    public int Count { get; set; }
}
