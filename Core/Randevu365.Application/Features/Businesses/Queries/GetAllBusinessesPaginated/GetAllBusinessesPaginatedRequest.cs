using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Queries.GetAllBusinessesPaginated;

public class GetAllBusinessesPaginatedRequest : IRequest<ApiResponse<PaginatedListDto<GetAllBusinessesPaginatedResponse>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
