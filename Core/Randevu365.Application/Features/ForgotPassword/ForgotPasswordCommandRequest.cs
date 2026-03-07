using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.ForgotPassword;

public class ForgotPasswordCommandRequest : IRequest<ApiResponse>
{
    public required string Email { get; set; }
}
