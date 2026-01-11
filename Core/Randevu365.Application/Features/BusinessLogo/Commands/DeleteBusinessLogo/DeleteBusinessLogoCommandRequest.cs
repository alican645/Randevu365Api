using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessLogo.Commands.DeleteBusinessLogo;

public class DeleteBusinessLogoCommandRequest : IRequest<ApiResponse<DeleteBusinessLogoCommandResponse>>
{
    public int BusinessId { get; set; }
}
