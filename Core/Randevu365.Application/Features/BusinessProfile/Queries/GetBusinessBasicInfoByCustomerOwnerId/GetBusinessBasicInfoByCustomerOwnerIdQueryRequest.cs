using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessBasicInfoByCustomerOwnerId;

public class GetBusinessBasicInfoByCustomerOwnerIdQueryRequest : IRequest<ApiResponse<GetBusinessBasicInfoByCustomerOwnerIdQueryResponse>>
{
    // Token'dan alınacak
}
