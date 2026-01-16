using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessHours.Commands.UpdateBusinessHour;

public class UpdateBusinessHourCommandHandler : IRequestHandler<UpdateBusinessHourCommandRequest, ApiResponse<UpdateBusinessHourCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusinessHourCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<UpdateBusinessHourCommandResponse>> Handle(UpdateBusinessHourCommandRequest request, CancellationToken cancellationToken)
    {

        var validator = new UpdateBusinessHourCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<UpdateBusinessHourCommandResponse>.FailResult(errors);
        }

        var businessHour = await _unitOfWork.GetReadRepository<BusinessHour>().GetAsync(x => x.Id == request.Id);
        if (businessHour == null)
        {
            return ApiResponse<UpdateBusinessHourCommandResponse>.NotFoundResult("Business hour not found.");
        }

        businessHour.Day = request.Day;
        businessHour.OpenTime = request.OpenTime;
        businessHour.CloseTime = request.CloseTime;

        await _unitOfWork.GetWriteRepository<BusinessHour>().UpdateAsync(businessHour);
        await _unitOfWork.SaveAsync();

        return ApiResponse<UpdateBusinessHourCommandResponse>.SuccessResult(new UpdateBusinessHourCommandResponse(), "Business hour updated successfully.");
    }
}
