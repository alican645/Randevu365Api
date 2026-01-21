using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessPhotos.Commands.DeleteBusinessPhoto;

public class DeleteBusinessPhotoCommandHandler : IRequestHandler<DeleteBusinessPhotoCommandRequest,
    ApiResponse<DeleteBusinessPhotoCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusinessPhotoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<DeleteBusinessPhotoCommandResponse>> Handle(DeleteBusinessPhotoCommandRequest request,
        CancellationToken cancellationToken)
    {
       
        var validator = new DeleteBusinessPhotoCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<DeleteBusinessPhotoCommandResponse>.FailResult(errors);
        }

        var photo = await _unitOfWork.GetReadRepository<BusinessPhoto>().GetAsync(x =>
            x.Id == request.BusinessPhotoId &&
            x.BusinessId == request.BusinessId);

        if (photo == null)
        {
            return ApiResponse<DeleteBusinessPhotoCommandResponse>.NotFoundResult("Fotoğraf bulunamadı.");
        }

        await _unitOfWork.GetWriteRepository<BusinessPhoto>().HardDeleteAsync(photo);
        await _unitOfWork.SaveAsync();

        return ApiResponse<DeleteBusinessPhotoCommandResponse>.SuccessResult(
            new DeleteBusinessPhotoCommandResponse { Id = request.BusinessPhotoId },
            "Fotoğraf başarıyla silindi.");
    }
}