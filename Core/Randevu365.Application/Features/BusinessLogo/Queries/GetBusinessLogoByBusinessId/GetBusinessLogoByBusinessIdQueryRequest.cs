using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessLogo.Queries.GetBusinessLogoByBusinessId;

public class GetBusinessLogoByBusinessIdQueryRequest : IRequest<ApiResponse<GetBusinessLogoByBusinessIdQueryResponse>>
{
    public int BusinessId { get; set; }
}
