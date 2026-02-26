using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Features.Businesses.Queries.GetBusinessesAllCategory;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessesByBusinessCategory;

public class GetBusinessesByBusinessCategoryRequest : IRequest<ApiResponse<PaginatedListDto<GetBusinessesByBusinessCategoryResponse>>>
{
    public string CategoryName { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
