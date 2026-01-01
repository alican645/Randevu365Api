using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessPhotos.Commands.CreateBusinessPhoto;

public class CreateBusinessPhotoCommandRequest : IRequest<ApiResponse<CreateBusinessPhotoCommandResponse>>
{
    public int BusinessId { get; set; }
    public string? PhotoPath { get; set; }
    public bool IsActive { get; set; }
}
