using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Businesses.Commands.UpdateBusiness;

public class UpdateBusinessCommandHandler : IRequestHandler<UpdateBusinessCommandRequest, ApiResponse<UpdateBusinessCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusinessCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<UpdateBusinessCommandResponse>> Handle(UpdateBusinessCommandRequest request, CancellationToken cancellationToken)
    {
        var businessResult = await _unitOfWork.GetReadRepository<Business>().GetAsync(x => x.Id == request.Id);

        if (businessResult == null)
        {
            return ApiResponse<UpdateBusinessCommandResponse>.NotFoundResult("İşyeri bulunamadı.");
        }

        businessResult.BusinessName = request.BusinessName;
        businessResult.BusinessAddress = request.BusinessAddress;
        businessResult.BusinessCity = request.BusinessCity;
        businessResult.BusinessPhone = request.BusinessPhone;
        businessResult.BusinessEmail = request.BusinessEmail;
        businessResult.BusinessCountry = request.BusinessCountry;

        await _unitOfWork.GetWriteRepository<Business>().UpdateAsync(businessResult);
        await _unitOfWork.SaveAsync();

        return ApiResponse<UpdateBusinessCommandResponse>.SuccessResult(new UpdateBusinessCommandResponse { Id = businessResult.Id }, "İşyeri başarıyla güncellendi.");
    }
}
