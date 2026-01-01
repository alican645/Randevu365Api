using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessById;

public class GetBusinessByIdQueryRequest : IRequest<ApiResponse<GetBusinessByIdQueryResponse>>
{
    public int Id { get; set; }
}
