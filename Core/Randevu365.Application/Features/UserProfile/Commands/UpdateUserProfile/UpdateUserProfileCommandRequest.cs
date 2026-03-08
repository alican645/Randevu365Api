using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.UserProfile.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandRequest : IRequest<ApiResponse<UpdateUserProfileCommandResponse>>
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? PhoneNumber { get; set; }
}
