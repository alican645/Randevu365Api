using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.ResetPassword;

public class ResetPasswordCommandRequest : IRequest<ApiResponse>
{
    public required string Email { get; set; }
    public required string ResetToken { get; set; }
    public required string NewPassword { get; set; }
}
