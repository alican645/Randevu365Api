using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Login;

public record LoginRequest : IRequest<ApiResponse<LoginResponse>>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
