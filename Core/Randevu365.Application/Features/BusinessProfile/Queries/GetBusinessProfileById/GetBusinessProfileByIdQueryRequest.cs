using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessProfileById;

public class GetBusinessProfileByIdQueryRequest : IRequest<ApiResponse<GetBusinessProfileByIdQueryResponse>>
{
    public int Id { get; set; }
}
