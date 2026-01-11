using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Entities = Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessLogo.Commands.DeleteBusinessLogo;

public class DeleteBusinessLogoCommandHandler : IRequestHandler<DeleteBusinessLogoCommandRequest, ApiResponse<DeleteBusinessLogoCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusinessLogoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<DeleteBusinessLogoCommandResponse>> Handle(DeleteBusinessLogoCommandRequest request, CancellationToken cancellationToken)
    {
        var businessLogo = await _unitOfWork.GetReadRepository<Entities.BusinessLogo>().GetAsync(x => x.BusinessId == request.BusinessId);
        if (businessLogo == null)
        {
            return ApiResponse<DeleteBusinessLogoCommandResponse>.NotFoundResult("Business logo not found.");
        }

        await _unitOfWork.GetWriteRepository<Entities.BusinessLogo>().HardDeleteAsync(businessLogo);
        await _unitOfWork.SaveAsync();

        return ApiResponse<DeleteBusinessLogoCommandResponse>.SuccessResult(new DeleteBusinessLogoCommandResponse(), "Business logo deleted successfully.");
    }
}
