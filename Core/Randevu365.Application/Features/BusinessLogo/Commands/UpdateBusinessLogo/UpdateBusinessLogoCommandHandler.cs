using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Entities = Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessLogo.Commands.UpdateBusinessLogo;

public class UpdateBusinessLogoCommandHandler : IRequestHandler<UpdateBusinessLogoCommandRequest, ApiResponse<UpdateBusinessLogoCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusinessLogoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<UpdateBusinessLogoCommandResponse>> Handle(UpdateBusinessLogoCommandRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateBusinessLogoCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<UpdateBusinessLogoCommandResponse>.FailResult(errors);
        }

        var businessLogo = await _unitOfWork.GetReadRepository<Entities.BusinessLogo>().GetAsync(x => x.BusinessId == request.BusinessId);
        if (businessLogo == null)
        {
            return ApiResponse<UpdateBusinessLogoCommandResponse>.NotFoundResult("Business logo not found.");
        }

        businessLogo.LogoUrl = request.LogoUrl;

        await _unitOfWork.GetWriteRepository<Entities.BusinessLogo>().UpdateAsync(businessLogo);
        await _unitOfWork.SaveAsync();

        return ApiResponse<UpdateBusinessLogoCommandResponse>.SuccessResult(new UpdateBusinessLogoCommandResponse { LogoUrl = businessLogo.LogoUrl }, "Business logo updated successfully.");
    }
}
