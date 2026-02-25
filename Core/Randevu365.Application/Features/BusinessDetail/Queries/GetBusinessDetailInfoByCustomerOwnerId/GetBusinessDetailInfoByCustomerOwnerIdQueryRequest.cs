using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessDetailInfoByCustomerOwnerId;

public class GetBusinessDetailInfoByCustomerOwnerIdQueryRequest : IRequest<ApiResponse<GetBusinessDetailInfoByCustomerOwnerIdQueryResponse>>
{
    // Token'dan alınacak
}
