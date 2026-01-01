using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessPhotos.Commands.DeleteBusinessPhoto;

public class DeleteBusinessPhotoCommandHandler : IRequestHandler<DeleteBusinessPhotoCommandRequest, ApiResponse<DeleteBusinessPhotoCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusinessPhotoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<DeleteBusinessPhotoCommandResponse>> Handle(DeleteBusinessPhotoCommandRequest request, CancellationToken cancellationToken)
    {
        var photo = await _unitOfWork.GetReadRepository<BusinessPhoto>().GetAsync(x => x.Id == request.Id);

        if (photo == null)
        {
            return ApiResponse<DeleteBusinessPhotoCommandResponse>.NotFoundResult("Business photo not found.");
        }

        await _unitOfWork.GetWriteRepository<BusinessPhoto>().HardDeleteAsync(photo);
        await _unitOfWork.SaveAsync();

        return ApiResponse<DeleteBusinessPhotoCommandResponse>.SuccessResult(new DeleteBusinessPhotoCommandResponse { Id = request.Id }, "Business photo deleted successfully.");
    }
}
