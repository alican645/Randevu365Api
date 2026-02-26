using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessesAllCategory;

public class GetBusinessesAllCategoryRequest : IRequest<ApiResponse<PaginatedListDto<GetBusinessesAllCategoryResponse>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
