using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessProfileByUserId;

public class GetBusinessProfileByUserIdQueryRequest : IRequest<ApiResponse<GetBusinessProfileByUserIdQueryResponse>>
{
    public int UserId { get; set; }
}
