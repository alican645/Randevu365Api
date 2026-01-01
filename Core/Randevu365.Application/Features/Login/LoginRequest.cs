using System.ComponentModel;
using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Login;

public record LoginRequest : IRequest<ApiResponse<LoginResponse>>
{
    [DefaultValue("alican@gmail.com")]
    public required string Email { get; set; }
    [DefaultValue("Alican123.")]
    public required string Password { get; set; }
}
