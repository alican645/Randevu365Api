using MediatR;
using Microsoft.AspNetCore.Http;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessPhotos.Commands.CreateBusinessPhoto;

public class CreateBusinessPhotoCommandRequest : IRequest<ApiResponse<CreateBusinessPhotoCommandResponse>>
{
    public int BusinessId { get; set; }
    public IFormFile? Photo { get; set; }
    public bool IsActive { get; set; } = true;
}
