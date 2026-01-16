using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessHours.Commands.DeleteBusinessHour;

public class DeleteBusinessHourCommandHandler : IRequestHandler<DeleteBusinessHourCommandRequest, ApiResponse<DeleteBusinessHourCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusinessHourCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<DeleteBusinessHourCommandResponse>> Handle(DeleteBusinessHourCommandRequest request, CancellationToken cancellationToken)
    {

        var validator = new DeleteBusinessHourCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<DeleteBusinessHourCommandResponse>.FailResult(errors);
        }

        var businessHour = await _unitOfWork.GetReadRepository<BusinessHour>().GetAsync(x => x.Id == request.Id);
        if (businessHour == null)
        {
            return ApiResponse<DeleteBusinessHourCommandResponse>.NotFoundResult("Business hour not found.");
        }

        await _unitOfWork.GetWriteRepository<BusinessHour>().HardDeleteAsync(businessHour);
        await _unitOfWork.SaveAsync();

        return ApiResponse<DeleteBusinessHourCommandResponse>.SuccessResult(new DeleteBusinessHourCommandResponse(), "Business hour deleted successfully.");
    }
}
