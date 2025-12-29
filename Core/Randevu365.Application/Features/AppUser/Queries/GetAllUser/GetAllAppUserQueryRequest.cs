
using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.AppUser.Queries.GetAllUser;

public class GetAllAppUserQueryRequest : IRequest<ApiResponse<IList<GetAllAppUserQueryResponse>>>
{
}