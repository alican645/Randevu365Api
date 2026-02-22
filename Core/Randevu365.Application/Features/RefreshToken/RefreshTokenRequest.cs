using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.RefreshToken;

public class RefreshTokenRequest : IRequest<ApiResponse<RefreshTokenResponse>>
{
    public required string RefreshToken { get; set; }
}
