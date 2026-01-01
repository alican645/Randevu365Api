using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Queries.GetAllBusinesses;

public class GetAllBusinessesQueryRequest : IRequest<ApiResponse<IList<GetAllBusinessesQueryResponse>>>
{
}
