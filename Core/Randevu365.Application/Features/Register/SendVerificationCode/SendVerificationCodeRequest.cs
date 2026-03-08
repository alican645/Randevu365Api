using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Register.SendVerificationCode;

public record SendVerificationCodeRequest : IRequest<ApiResponse>
{
    public required string Email { get; set; }
}
