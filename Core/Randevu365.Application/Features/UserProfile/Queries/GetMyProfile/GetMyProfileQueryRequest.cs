using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.UserProfile.Queries.GetMyProfile;

public class GetMyProfileQueryRequest : IRequest<ApiResponse<GetMyProfileQueryResponse>>
{
}
