using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessLogo.Commands.UpdateBusinessLogo;

public class UpdateBusinessLogoCommandRequest : IRequest<ApiResponse<UpdateBusinessLogoCommandResponse>>
{
    public int BusinessId { get; set; }
    public required string LogoUrl { get; set; }
}
