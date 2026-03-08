
using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Register;

public abstract record RegisterRequest : IRequest<ApiResponse<RegisterResponse>>
{
    public required string Email { get; set; }
    public required string Password { get; set; }

    // AppUserInformation properties
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string PhoneNumber { get; set; }
    public required string VerificationCode { get; set; }
}

public record RegisterCustomerRequest : RegisterRequest;

public record RegisterBusinessOwnerRequest : RegisterRequest;