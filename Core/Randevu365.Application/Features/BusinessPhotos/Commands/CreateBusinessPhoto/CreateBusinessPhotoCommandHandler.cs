using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessPhotos.Commands.CreateBusinessPhoto;

public class CreateBusinessPhotoCommandHandler : IRequestHandler<CreateBusinessPhotoCommandRequest, ApiResponse<CreateBusinessPhotoCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;

    public CreateBusinessPhotoCommandHandler(IUnitOfWork unitOfWork, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<ApiResponse<CreateBusinessPhotoCommandResponse>> Handle(CreateBusinessPhotoCommandRequest request, CancellationToken cancellationToken)
    {

        var validator = new CreateBusinessPhotoCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<CreateBusinessPhotoCommandResponse>.FailResult(errors);
        }

        var isFull = await _unitOfWork.GetReadRepository<BusinessPhoto>().CountAsync(b => b.BusinessId == request.BusinessId);
        if (isFull >= 5)
        {
            return ApiResponse<CreateBusinessPhotoCommandResponse>.FailResult("Bir işletme için en fazla 5 fotoğraf olabilir.");
        }

        var photoPath = await _fileService.UploadFileAsync(request.Photo, $"business/{request.BusinessId}/photos");

        var photo = new BusinessPhoto
        {
            BusinessId = request.BusinessId,
            PhotoPath = photoPath,
            IsActive = request.IsActive
        };

        await _unitOfWork.GetWriteRepository<BusinessPhoto>().AddAsync(photo);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CreateBusinessPhotoCommandResponse>.SuccessResult(
            new CreateBusinessPhotoCommandResponse { Id = photo.Id, PhotoPath = photoPath },
            "Business photo added successfully.");
    }
}
