using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetCustomerBusinessProfile;

public class GetCustomerBusinessProfileQueryRequest : IRequest<ApiResponse<GetCustomerBusinessProfileQueryResponse>>
{
    public int BusinessId { get; set; }
}
