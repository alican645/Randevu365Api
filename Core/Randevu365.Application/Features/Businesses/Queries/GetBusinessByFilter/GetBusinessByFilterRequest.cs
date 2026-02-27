using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessByFilter;

public class GetBusinessByFilterRequest : IRequest<ApiResponse<PaginatedListDto<GetBusinessByFilterResponse>>>
{
    public string? Category { get; set; }
    public string? QueryParam { get; set; }
    public bool? OrderByRating { get; set; }
    public bool? OrderByCommentCount { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
