using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.UserProfile.Commands.ChangePassword;

public class ChangePasswordCommandRequest : IRequest<ApiResponse>
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
