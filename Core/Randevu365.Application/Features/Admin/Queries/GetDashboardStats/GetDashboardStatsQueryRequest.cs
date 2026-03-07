using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsQueryRequest : IRequest<ApiResponse<GetDashboardStatsQueryResponse>>
{
}
