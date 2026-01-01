using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessPhotos.Commands.CreateBusinessPhoto;

public class CreateBusinessPhotoCommandHandler : IRequestHandler<CreateBusinessPhotoCommandRequest, ApiResponse<CreateBusinessPhotoCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusinessPhotoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CreateBusinessPhotoCommandResponse>> Handle(CreateBusinessPhotoCommandRequest request, CancellationToken cancellationToken)
    {
        var photo = new BusinessPhoto
        {
            BusinessId = request.BusinessId,
            PhotoPath = request.PhotoPath,
            IsActive = request.IsActive
        };

        await _unitOfWork.GetWriteRepository<BusinessPhoto>().AddAsync(photo);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CreateBusinessPhotoCommandResponse>.SuccessResult(new CreateBusinessPhotoCommandResponse { Id = photo.Id }, "Business photo added successfully.");
    }
}
