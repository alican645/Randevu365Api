using MediatR;
using Microsoft.AspNetCore.Http;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessLogo.Commands.CreateBusinessLogo;

public class CreateBusinessLogoCommandRequest : IRequest<ApiResponse<CreateBusinessLogoCommandResponse>>
{
    public int BusinessId { get; set; }
    public IFormFile? Logo { get; set; }
}
