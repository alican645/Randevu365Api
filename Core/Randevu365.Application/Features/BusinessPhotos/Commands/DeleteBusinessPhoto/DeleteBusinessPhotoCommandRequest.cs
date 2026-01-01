using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessPhotos.Commands.DeleteBusinessPhoto;

public class DeleteBusinessPhotoCommandRequest : IRequest<ApiResponse<DeleteBusinessPhotoCommandResponse>>
{
    public int Id { get; set; }
}
