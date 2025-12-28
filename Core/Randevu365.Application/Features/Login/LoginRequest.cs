using MediatR;

namespace Randevu365.Application.Features.Login;

public record LoginRequest : IRequest<LoginResponse>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
