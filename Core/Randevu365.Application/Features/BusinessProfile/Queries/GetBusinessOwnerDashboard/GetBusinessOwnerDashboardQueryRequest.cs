using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessOwnerDashboard;

public class GetBusinessOwnerDashboardQueryRequest : IRequest<ApiResponse<GetBusinessOwnerDashboardQueryResponse>>
{
    // Parametresiz — kimlik JWT'den alınır
}
