using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Entities = Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessLogo.Commands.CreateBusinessLogo;

public class CreateBusinessLogoCommandHandler : IRequestHandler<CreateBusinessLogoCommandRequest, ApiResponse<CreateBusinessLogoCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusinessLogoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CreateBusinessLogoCommandResponse>> Handle(CreateBusinessLogoCommandRequest request, CancellationToken cancellationToken)
    {
        // Check if logo already exists for this business
        var existingLogo = await _unitOfWork.GetReadRepository<Entities.BusinessLogo>().GetAsync(x => x.BusinessId == request.BusinessId);
        if (existingLogo != null)
        {
            return ApiResponse<CreateBusinessLogoCommandResponse>.FailResult("A logo already exists for this business. Please update the existing logo.");
        }

        var businessLogo = new Entities.BusinessLogo
        {
            BusinessId = request.BusinessId,
            LogoUrl = request.LogoUrl
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
