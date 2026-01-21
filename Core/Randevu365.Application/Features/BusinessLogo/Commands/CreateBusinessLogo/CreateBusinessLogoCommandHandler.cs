using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Entities = Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessLogo.Commands.CreateBusinessLogo;

public class CreateBusinessLogoCommandHandler : IRequestHandler<CreateBusinessLogoCommandRequest, ApiResponse<CreateBusinessLogoCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;

    public CreateBusinessLogoCommandHandler(IUnitOfWork unitOfWork, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<ApiResponse<CreateBusinessLogoCommandResponse>> Handle(CreateBusinessLogoCommandRequest request, CancellationToken cancellationToken)
    {

        var validator = new CreateBusinessLogoCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<CreateBusinessLogoCommandResponse>.FailResult(errors);
        }

        
        var existingLogo = await _unitOfWork.GetReadRepository<Entities.BusinessLogo>().GetAsync(x => x.BusinessId == request.BusinessId);
        if (existingLogo != null)
        {
            return ApiResponse<CreateBusinessLogoCommandResponse>.ConflictResult("A logo already exists for this business. Please update the existing logo.");
        }

        var logoUrl = await _fileService.UploadFileAsync(request.Logo, $"business/{request.BusinessId}/logo");

        var businessLogo = new Entities.BusinessLogo
        {
            BusinessId = request.BusinessId,
            LogoUrl = logoUrl
        };

        await _unitOfWork.GetWriteRepository<Entities.BusinessLogo>().AddAsync(businessLogo);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CreateBusinessLogoCommandResponse>.SuccessResult(new CreateBusinessLogoCommandResponse
        {
            Id = businessLogo.Id,
            LogoUrl = businessLogo.LogoUrl
        }, "Business logo created successfully.");
    }
}
