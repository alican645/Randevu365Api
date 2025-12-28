
using MediatR;

namespace Randevu365.Application.Features.Register;

public record RegisterRequest : IRequest<RegisterResponse>
{
    // AppUser properties
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required int Role { get; set; }

    // AppUserInformation properties
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required int Age { get; set; }
    public required string Gender { get; set; }
    public required string PhoneNumber { get; set; }
    public required int Height { get; set; }
    public required int Weight { get; set; }
}